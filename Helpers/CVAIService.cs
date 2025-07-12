using InsanK.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace InsanK.Helpers
{
    public class CVAIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private readonly string _apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        private readonly string _altApiUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent";
        private readonly bool _useProxy = false;
        private readonly string _proxyUrl = "https://cors-anywhere.herokuapp.com/";
        private readonly bool _useFallbackMode = false;
        private readonly ILogger<CVAIService> _logger;

        public CVAIService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<CVAIService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["AISettings:AccessToken"];
            _logger = logger;
                
            _useProxy = configuration.GetValue<bool>("AISettings:UseProxy", false);
            _proxyUrl = configuration.GetValue<string>("AISettings:ProxyUrl", "https://cors-anywhere.herokuapp.com/");
        }

        public async Task<List<CVOneri>> AnalizEtVeOneriGetir(CV cv, string soru = "")
        {
            try
            {
                string promptMessage = $@"
Sen bir İnsan Kaynakları ve Kariyer Danışmanısın. Aşağıdaki CV bilgilerini analiz ederek kişiye kariyer tavsiyeleri vermeni istiyorum.

CV Bilgileri:
- Meslek: {cv.Meslek ?? "Belirtilmemiş"}
- Hedef Pozisyon: {cv.HedefPozisyon ?? "Belirtilmemiş"}
- Hedef Şirket: {cv.HedefSirket ?? "Belirtilmemiş"}
- Özet: {cv.Ozet ?? "Belirtilmemiş"}
- Eğitim: {cv.Egitim ?? "Belirtilmemiş"}
- Deneyim: {cv.Deneyim ?? "Belirtilmemiş"}
- Beceriler: {cv.Beceriler ?? "Belirtilmemiş"}
- Sertifikalar: {cv.Sertifikalar ?? "Belirtilmemiş"}
- Diller: {cv.Diller ?? "Belirtilmemiş"}
- Referanslar: {cv.Referanslar ?? "Belirtilmemiş"}
- Hobiler: {cv.Hobiler ?? "Belirtilmemiş"}

{(string.IsNullOrEmpty(soru) ? "" : $"Kullanıcının özel sorusu: {soru}\n\n")}

Lütfen kişiye 5 adet tavsiye ver. Her tavsiye için şu formatı kullan:
1. Tavsiye Başlığı: [kısa ve net bir başlık]
2. Açıklama: [Detaylı açıklama, 100-200 kelime arası]
3. Kategori: [Eğitim, Deneyim, Beceriler, Kişisel Gelişim]

Tavsiyeleri öncelik sırasına göre listele.";

                var requestData = new
                {
                    contents = new[]
                    {
                        new
                        {
                            role = "user",
                            parts = new[]
                            {
                                new { text = promptMessage }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 800,
                        topK = 40, 
                        topP = 0.95
                    }
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var client = _httpClientFactory.CreateClient("GeminiClient");
                
                var requestUrl = _useProxy ? $"{_proxyUrl}{_apiUrl}?key={_apiKey}" : $"{_apiUrl}?key={_apiKey}";
                _logger.LogInformation("Sending request to Gemini API. URL format: {ApiUrl}", _apiUrl);
                _logger.LogInformation("Request JSON: {RequestJson}", json);
                
                try {
                    var testModelsUrl = "https://generativelanguage.googleapis.com/v1beta/models?key=" + _apiKey;
                    var testResponse = await client.GetAsync(testModelsUrl);
                    _logger.LogInformation("Kullanılabilir modelleri test e: {StatusCode}", testResponse.StatusCode);
                    
                    if (testResponse.IsSuccessStatusCode) {
                        var modelContent = await testResponse.Content.ReadAsStringAsync();
                        _logger.LogInformation("Kullanılabilir modeller: {Models}", modelContent);
                    }
                } catch (Exception ex) {
                    _logger.LogError(ex, "Test bağlantısı başarısız: {Message}", ex.Message);
                    throw new InvalidOperationException("Bağlantı testi başarısız: " + ex.Message, ex);
                }
                
                var response = await client.PostAsync(requestUrl, content);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Ana URL 404 hatası döndürdü. Alternatif URL deneniyor: {AltUrl}", _altApiUrl);
                    var altRequestUrl = _useProxy ? $"{_proxyUrl}{_altApiUrl}?key={_apiKey}" : $"{_altApiUrl}?key={_apiKey}";
                    response = await client.PostAsync(altRequestUrl, content);
                    _logger.LogInformation("Alternatif URL yanıtı: {StatusCode}", response.StatusCode);
                }

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Başarılı yanıt içeriği: {ResponseContent}", responseContent);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var apiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent, options);

                    if (apiResponse?.candidates != null && apiResponse.candidates.Length > 0 &&
                        apiResponse.candidates[0].content?.parts != null && apiResponse.candidates[0].content.parts.Length > 0)
                    {
                        var responseText = apiResponse.candidates[0].content.parts[0].text;
                        var cvoneriler = AyristirTavsiyeler(responseText, cv.Id);

                        cv.AIAnalizi = responseText;

                        return cvoneriler;
                    }
                    else
                    {
                        _logger.LogWarning("API yanıtı başarılı ancak candidates içeriği uygun değil: {ResponseContent}", responseContent);
                        throw new InvalidOperationException($"API yanıtı düzgün formatta değil: {responseContent}");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API hata içeriği: {ErrorContent}", errorContent);
                    _logger.LogWarning("API yanıtı başarısız. StatusCode: {StatusCode}", response.StatusCode);
                    throw new InvalidOperationException($"Google API hatası: {response.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Google API'sine bağlanamıyor: {Message}", ex.Message);
                throw new InvalidOperationException("Google API'sine bağlantı sağlanamıyor: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API hatası: {Message}", ex.Message);
                throw new InvalidOperationException("CV analizi sırasında hata oluştu: " + ex.Message, ex);
            }
        }

        public async Task<string> CVIlanOneriOlustur(CV cv, Ilan ilan)
        {
            try
            {
                if (cv == null || ilan == null)
                {
                    _logger.LogWarning("CV veya ilan null");
                    throw new InvalidOperationException("CV veya ilan bulunamadı.");
                }

                _logger.LogInformation($"CV-İlan karşılaştırma analizi başlıyor. CV ID: {cv.Id}, İlan ID: {ilan.Id}");

                var cvContent = new StringBuilder();
                bool cvIcerikVar = false;

                if (!string.IsNullOrEmpty(cv.Baslik))
                {
                    cvContent.AppendLine($"Özgeçmiş Başlık: {cv.Baslik}");
                    cvIcerikVar = true;
                }

                if (!string.IsNullOrEmpty(cv.Ozet))
                {
                    cvContent.AppendLine($"Özet: {cv.Ozet}");
                    cvIcerikVar = true;
                }

                if (!string.IsNullOrEmpty(cv.Meslek))
                {
                    cvContent.AppendLine($"Meslek: {cv.Meslek}");
                    cvIcerikVar = true;
                }

                if (!string.IsNullOrEmpty(cv.Egitim))
                {
                    cvContent.AppendLine($"Eğitim: {cv.Egitim}");
                    cvIcerikVar = true;
                }

                if (!string.IsNullOrEmpty(cv.Deneyim))
                {
                    cvContent.AppendLine($"Deneyim: {cv.Deneyim}");
                    cvIcerikVar = true;
                }

                if (!string.IsNullOrEmpty(cv.Beceriler))
                {
                    cvContent.AppendLine($"Beceriler: {cv.Beceriler}");
                    cvIcerikVar = true;
                }

                if (!string.IsNullOrEmpty(cv.Sertifikalar))
                {
                    cvContent.AppendLine($"Sertifikalar: {cv.Sertifikalar}");
                    cvIcerikVar = true;
                }

                if (!string.IsNullOrEmpty(cv.Diller))
                {
                    cvContent.AppendLine($"Diller: {cv.Diller}");
                    cvIcerikVar = true;
                }

                var ilanContent = new StringBuilder();
                ilanContent.AppendLine($"İlan Başlığı: {ilan.Baslik}");
                ilanContent.AppendLine($"Lokasyon: {ilan.Lokasyon}");

                string sirketAdi = ilan.Kullanici != null ? ilan.Kullanici.KullaniciAdi : "Belirtilmemiş";
                ilanContent.AppendLine($"Şirket: {sirketAdi}");

                if (!string.IsNullOrEmpty(ilan.Aciklama))
                {
                    ilanContent.AppendLine($"İlan Açıklaması: {ilan.Aciklama}");
                }

                if (!string.IsNullOrEmpty(ilan.GerekliYetkinlikler))
                {
                    ilanContent.AppendLine($"Gerekli Yetkinlikler: {ilan.GerekliYetkinlikler}");
                }

                if (!string.IsNullOrEmpty(ilan.TecrubeSeviyesi))
                {
                    ilanContent.AppendLine($"Tecrübe Seviyesi: {ilan.TecrubeSeviyesi}");
                }

                if (!string.IsNullOrEmpty(ilan.PozisyonTipi))
                {
                    ilanContent.AppendLine($"Pozisyon Tipi: {ilan.PozisyonTipi}");
                }

                if (!cvIcerikVar)
                {
                    _logger.LogWarning($"CV {cv.Id} için içerik bulunamadı");
                    throw new InvalidOperationException("CV içeriği olmadığı için öneri yapamıyorum. Lütfen CV içeriğini doldurun.");
                }

                var promptMessage = $@"Lütfen aşağıdaki CV'yi ve iş ilanını karşılaştır ve bu adayın ilana başvurması için CV'sini nasıl geliştirebileceği konusunda detaylı öneriler sun. Ayrıca CV'nin bu iş ilanı için ne kadar uygun olduğunu değerlendir.                

                CV İçeriği:
{cvContent}

                İş İlanı:
{ilanContent}

                Şunları içeren detaylı bir yanıt hazırla:
                1. CV'nin bu iş ilanına uygunluğunun genel değerlendirmesi
                2. İş ilanındaki gereksinimleri karşılamada CV'nin güçlü ve zayıf yanları
                3. CV'nin bu iş ilanına daha uygun hale getirilmesi için özel öneriler
                4. CV'ye eklenebilecek anahtar kelimeler ve beceriler
                5. CV'nin hangi bölümlerinin vurgulanması veya öne çıkarılması gerektiği
                
                Yanıtını Türkçe olarak yaz ve mümkün olduğunca spesifik ve somut öneriler ver.";

                var requestData = new
                {
                    contents = new[]
                    {
                        new
                        {
                            role = "user",
                            parts = new[]
                            {
                                new { text = promptMessage }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 2000,
                        topK = 40,
                        topP = 0.95
                    }
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var requestUrl = _useProxy ? $"{_proxyUrl}{_apiUrl}?key={_apiKey}" : $"{_apiUrl}?key={_apiKey}";
                var client = _httpClientFactory.CreateClient("GeminiClient");
                var response = await client.PostAsync(requestUrl, content);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Ana URL 404 hatası döndürdü. Alternatif URL deneniyor: {AltUrl}", _altApiUrl);
                    var altRequestUrl = _useProxy ? $"{_proxyUrl}{_altApiUrl}?key={_apiKey}" : $"{_altApiUrl}?key={_apiKey}";
                    response = await client.PostAsync(altRequestUrl, content);
                    _logger.LogInformation("Alternatif URL yanıtı: {StatusCode}", response.StatusCode);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Google AI API çağrısı başarısız oldu: {response.StatusCode}, Hata: {errorContent}");
                    throw new InvalidOperationException($"AI servisi şu anda yanıt veremiyor: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent, options);
                var aiText = geminiResponse?.candidates != null && geminiResponse.candidates.Length > 0 &&
                             geminiResponse.candidates[0].content?.parts != null && geminiResponse.candidates[0].content.parts.Length > 0
                             ? geminiResponse.candidates[0].content.parts[0].text
                             : null;

                if (string.IsNullOrEmpty(aiText))
                {
                    _logger.LogWarning("AI yanıtı boş veya null");
                    throw new InvalidOperationException("AI servisi boş bir yanıt döndü. Lütfen daha sonra tekrar deneyin.");
                }

                return aiText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CV-İlan karşılaştırma analizi sırasında hata oluştu.");
                throw new InvalidOperationException($"CV-İlan karşılaştırma analizi sırasında hata: {ex.Message}", ex);
            }
        }

        private List<CVOneri> AyristirTavsiyeler(string apiResponse, int cvId)
        {
            var oneriler = new List<CVOneri>();

            try
            {
                var tavsiyeBolumleri = new List<string>();

                if (apiResponse.Contains("1. Tavsiye Başlığı:"))
                {
                    var splits = apiResponse.Split(new[] { "1. Tavsiye Başlığı:", "2. Tavsiye Başlığı:", "3. Tavsiye Başlığı:", "4. Tavsiye Başlığı:", "5. Tavsiye Başlığı:" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 1; i < splits.Length && i <= 5; i++)
                    {
                        tavsiyeBolumleri.Add(splits[i]);
                    }
                }
                else if (apiResponse.Contains("Tavsiye 1:"))
                {
                    var splits = apiResponse.Split(new[] { "Tavsiye 1:", "Tavsiye 2:", "Tavsiye 3:", "Tavsiye 4:", "Tavsiye 5:" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 1; i < splits.Length && i <= 5; i++)
                    {
                        tavsiyeBolumleri.Add(splits[i]);
                    }
                }
                else
                {
                    var lines = apiResponse.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    string currentSection = "";

                    for (int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i].Trim();

                        if (line.StartsWith("1.") || line.StartsWith("2.") || line.StartsWith("3.") ||
                            line.StartsWith("4.") || line.StartsWith("5."))
                        {
                            if (!string.IsNullOrEmpty(currentSection))
                            {
                                tavsiyeBolumleri.Add(currentSection);
                                currentSection = "";
                            }

                            currentSection = line;
                        }
                        else if (!string.IsNullOrEmpty(currentSection))
                        {
                            currentSection += "\n" + line;
                        }
                    }

                    if (!string.IsNullOrEmpty(currentSection))
                    {
                        tavsiyeBolumleri.Add(currentSection);
                    }
                }

                for (int i = 0; i < tavsiyeBolumleri.Count && i < 5; i++)
                {
                    var tavsiye = tavsiyeBolumleri[i].Trim();

                    string baslik = "Tavsiye " + (i + 1);
                    string aciklama = tavsiye;
                    string kategori = "Genel";

                    if (tavsiye.Contains("Başlık:") || tavsiye.Contains("Tavsiye Başlığı:"))
                    {
                        int baslikBaslangic = tavsiye.IndexOf("Başlık:") != -1 ?
                            tavsiye.IndexOf("Başlık:") + 7 : tavsiye.IndexOf("Tavsiye Başlığı:") + 16;

                        int baslikBitis = tavsiye.IndexOf("Açıklama:");
                        if (baslikBitis == -1) baslikBitis = tavsiye.IndexOf("2. Açıklama:");
                        if (baslikBitis == -1) baslikBitis = tavsiye.IndexOf("\n", baslikBaslangic);
                        if (baslikBitis == -1) baslikBitis = tavsiye.Length;

                        if (baslikBitis > baslikBaslangic)
                        {
                            baslik = tavsiye.Substring(baslikBaslangic, baslikBitis - baslikBaslangic).Trim();
                        }

                        int aciklamaBaslangic = tavsiye.IndexOf("Açıklama:") != -1 ?
                            tavsiye.IndexOf("Açıklama:") + 9 : tavsiye.IndexOf("2. Açıklama:") + 11;

                        int aciklamaBitis = tavsiye.IndexOf("Kategori:");
                        if (aciklamaBitis == -1) aciklamaBitis = tavsiye.IndexOf("3. Kategori:");
                        if (aciklamaBitis == -1) aciklamaBitis = tavsiye.Length;

                        if (aciklamaBaslangic != -1 && aciklamaBitis > aciklamaBaslangic)
                        {
                            aciklama = tavsiye.Substring(aciklamaBaslangic, aciklamaBitis - aciklamaBaslangic).Trim();
                        }

                        int kategoriBaslangic = tavsiye.IndexOf("Kategori:") != -1 ?
                            tavsiye.IndexOf("Kategori:") + 9 : tavsiye.IndexOf("3. Kategori:") + 11;

                        if (kategoriBaslangic != -1 && kategoriBaslangic < tavsiye.Length)
                        {
                            kategori = tavsiye.Substring(kategoriBaslangic).Trim();
                        }
                    }

                    var oneri = new CVOneri
                    {
                        CVId = cvId,
                        Baslik = baslik.Length > 100 ? baslik.Substring(0, 97) + "..." : baslik,
                        Aciklama = aciklama.Length > 1000 ? aciklama.Substring(0, 997) + "..." : aciklama,
                        Kategori = kategori.Length > 50 ? kategori.Substring(0, 47) + "..." : kategori,
                        OncelikSirasi = i + 1,
                        OlusturmaTarihi = DateTime.Now
                    };

                    oneriler.Add(oneri);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tavsiye ayrıştırma hatası: {Message}", ex.Message);
                if (oneriler.Count == 0)
                {
                    oneriler.Add(new CVOneri
                    {
                        CVId = cvId,
                        Baslik = "CV Analizi",
                        Aciklama = "CV analizi yapıldı ancak öneriler oluşturulamadı. Lütfen daha sonra tekrar deneyin.",
                        Kategori = "Genel",
                        OncelikSirasi = 1,
                        OlusturmaTarihi = DateTime.Now
                    });
                }
            }

            return oneriler;
        }
        


        public class GeminiResponse
        {
            public Candidate[] candidates { get; set; } = Array.Empty<Candidate>();
            public string[] promptFeedback { get; set; } = Array.Empty<string>();
        }

        public class Candidate
        {
            public Content content { get; set; } = new Content();
            public string finishReason { get; set; } = string.Empty;
            public int? safetyRating { get; set; }
        }

        public class Content
        {
            public Part[] parts { get; set; } = Array.Empty<Part>();
            public string role { get; set; } = "model";
        }

        public class Part
        {
            public string text { get; set; } = string.Empty;
        }
    }
}