using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace InsanK.Helpers
{
    public class PDFExtractor
    {
        private readonly ILogger<PDFExtractor> _logger;

        public PDFExtractor(ILogger<PDFExtractor> logger)
        {
            _logger = logger;
        }

        public Dictionary<string, string> ExtractCVDataFromPDF(string filePath)
        {
            try
            {
                var result = new Dictionary<string, string>();
                var fullText = ExtractTextFromPDF(filePath);

                if (string.IsNullOrEmpty(fullText))
                {
                    _logger.LogWarning("PDF dosyasından metin çıkarılamadı: {FilePath}", filePath);
                    return result;
                }

                result["Ozet"] = TrimText(fullText.Substring(0, Math.Min(500, fullText.Length)));

                result["Egitim"] = ExtractSection(fullText, new[] { "eğitim", "education", "öğrenim", "okul" });
                result["Deneyim"] = ExtractSection(fullText, new[] { "deneyim", "experience", "iş tecrübesi", "çalışma" });
                result["Beceriler"] = ExtractSection(fullText, new[] { "beceri", "yetenek", "skills", "skill set" });
                result["Diller"] = ExtractSection(fullText, new[] { "dil", "yabancı dil", "languages" });
                result["Sertifikalar"] = ExtractSection(fullText, new[] { "sertifika", "certificate", "certification" });
                result["Referanslar"] = ExtractSection(fullText, new[] { "referans", "reference" });
                result["Hobiler"] = ExtractSection(fullText, new[] { "hobi", "ilgi alanları", "interests", "hobbies" });

                result["Meslek"] = ExtractJobTitle(fullText);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF içerik çıkarma sırasında hata oluştu: {FilePath}", filePath);
                return new Dictionary<string, string>();
            }
        }

        private string ExtractTextFromPDF(string filePath)
        {
            var text = new StringBuilder();

            try
            {
                using (var pdfReader = new PdfReader(filePath))
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    _logger.LogInformation("PDF document açıldı, sayfa sayısı: {PageCount}", pdfDocument.GetNumberOfPages());
                    
                    for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                    {
                        _logger.LogInformation("Sayfa {Page} okunuyor", i);
                        var page = pdfDocument.GetPage(i);
                        var strategy = new LocationTextExtractionStrategy();
                        var currentText = PdfTextExtractor.GetTextFromPage(page, strategy);
                        text.Append(currentText);
                        
                        _logger.LogInformation("Sayfa {Page} içeriği uzunluğu: {Length} karakter", i, currentText.Length);
                        if (currentText.Length > 0)
                        {
                            _logger.LogInformation("Sayfa {Page} örnek içerik: {Content}", i, 
                                currentText.Substring(0, Math.Min(100, currentText.Length)));
                        }
                    }
                    
                    _logger.LogInformation("Toplam çıkarılan içerik uzunluğu: {Length} karakter", text.Length);
                    if (text.Length > 0)
                    {
                        _logger.LogInformation("PDF içeriği örnek: {Content}", 
                            text.ToString().Substring(0, Math.Min(200, text.Length)));
                    }
                    else
                    {
                        _logger.LogWarning("PDF'den hiç metin çıkarılamadı");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF dosyası okunurken hata oluştu: {FilePath}", filePath);
            }

            return text.ToString();
        }

        private string ExtractSection(string fullText, string[] sectionKeywords)
        {
            fullText = fullText.ToLowerInvariant();
            foreach (var keyword in sectionKeywords)
            {
                var index = fullText.IndexOf(keyword.ToLowerInvariant());
                if (index >= 0)
                {
                    var start = index;
                    var length = Math.Min(1000, fullText.Length - start);
                    var section = fullText.Substring(start, length);

                    var nextSectionIndex = FindNextSectionIndex(section, sectionKeywords);
                    if (nextSectionIndex > 0)
                    {
                        section = section.Substring(0, nextSectionIndex);
                    }

                    return TrimText(section);
                }
            }

            return string.Empty;
        }

        private int FindNextSectionIndex(string text, string[] excludeKeywords)
        {
            var sectionHeaders = new[] 
            { 
                "eğitim", "education", "deneyim", "experience", "beceri", "skills",
                "dil", "languages", "sertifika", "certificate", "referans", "reference",
                "hobi", "interests", "iletişim", "contact", "profil", "profile"
            };

            var minIndex = int.MaxValue;
            
            foreach (var header in sectionHeaders)
            {
                if (excludeKeywords.Contains(header))
                    continue;

                var index = text.IndexOf(header, 50, StringComparison.OrdinalIgnoreCase);
                if (index > 0 && index < minIndex)
                {
                    minIndex = index;
                }
            }

            return minIndex == int.MaxValue ? -1 : minIndex;
        }

        private string ExtractJobTitle(string fullText)
        {
            var jobTitleRegexes = new[]
            {
                @"(?:position|pozisyon|title|ünvan|meslek)[\s:]*([^\n\.]+)",
                @"(?:senior|junior|lead|chief|başkan|müdür|uzman|mühendis|developer|designer|analyst)[^\n\.]+"
            };

            foreach (var regex in jobTitleRegexes)
            {
                var match = Regex.Match(fullText, regex, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    return TrimText(match.Groups[1].Value);
                }
            }

            return string.Empty;
        }

        private string TrimText(string text)
        {
            text = Regex.Replace(text, @"\s+", " ");
            return text.Trim();
        }
    }
}