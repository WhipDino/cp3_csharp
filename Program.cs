using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalyzer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Processador Assincrono de Arquivos de Texto ===");
            Console.WriteLine();

            try
            {
                // Solicitar diretório ao usuário
                string? directoryPath = GetDirectoryFromUser();
                if (string.IsNullOrEmpty(directoryPath))
                {
                    Console.WriteLine("Operação cancelada pelo usuário.");
                    return;
                }

                // Verificar se o diretório existe
                if (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine($"Erro: O diretório '{directoryPath}' não existe.");
                    return;
                }

                // Buscar arquivos de texto
                var textFiles = GetTextFiles(directoryPath);
                if (!textFiles.Any())
                {
                    Console.WriteLine($"Nenhum arquivo de texto encontrado em '{directoryPath}'.");
                    return;
                }

                Console.WriteLine($"\nEncontrados {textFiles.Count()} arquivos .txt:");
                foreach (var file in textFiles)
                {
                    Console.WriteLine($"  - {Path.GetFileName(file)}");
                }

                // Analisar arquivos de forma assíncrona
                Console.WriteLine("\nIniciando processamento dos arquivos...");
                var analysisResults = await AnalyzeFilesAsync(textFiles);

                // Gerar relatório consolidado
                var reportPath = GenerateReport(analysisResults);
                Console.WriteLine($"\nProcessamento concluído com sucesso!");
                Console.WriteLine($"Relatório gerado em: {reportPath}");

                // Exibir resumo na tela
                DisplaySummary(analysisResults);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro inesperado: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex}");
            }

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        static string? GetDirectoryFromUser()
        {
            Console.Write("Informe o caminho de um diretório contendo arquivos .txt: ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                return Environment.CurrentDirectory;
            }

            return input;
        }

        static IEnumerable<string> GetTextFiles(string directoryPath)
        {
            try
            {
                return Directory.GetFiles(directoryPath, "*.txt", SearchOption.TopDirectoryOnly).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar arquivos: {ex.Message}");
                return Enumerable.Empty<string>();
            }
        }

        static async Task<List<FileAnalysisResult>> AnalyzeFilesAsync(IEnumerable<string> filePaths)
        {
            var tasks = filePaths.Select(filePath => AnalyzeFileAsync(filePath));
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        static async Task<FileAnalysisResult> AnalyzeFileAsync(string filePath)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                Console.WriteLine($"Processando arquivo {fileName}...");

                var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
                
                var lineCount = content.Split('\n').Length;
                var wordCount = content.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                var charCount = content.Length;
                var fileSize = new FileInfo(filePath).Length;

                return new FileAnalysisResult
                {
                    FileName = fileName,
                    FilePath = filePath,
                    LineCount = lineCount,
                    WordCount = wordCount,
                    CharCount = charCount,
                    FileSizeBytes = fileSize,
                    AnalysisTime = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao analisar {Path.GetFileName(filePath)}: {ex.Message}");
                return new FileAnalysisResult
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    Error = ex.Message,
                    AnalysisTime = DateTime.Now
                };
            }
        }

        static string GenerateReport(List<FileAnalysisResult> results)
        {
            var reportDirectory = Path.Combine(Environment.CurrentDirectory, "export");
            Directory.CreateDirectory(reportDirectory);

            var reportPath = Path.Combine(reportDirectory, "relatorio.txt");

            var report = new StringBuilder();
            report.AppendLine("Arquivo - Linhas - Palavras");
            
            foreach (var result in results.OrderBy(r => r.FileName))
            {
                if (string.IsNullOrEmpty(result.Error))
                {
                    report.AppendLine($"{result.FileName} - {result.LineCount} linhas - {result.WordCount} palavras");
                }
            }

            File.WriteAllText(reportPath, report.ToString(), Encoding.UTF8);
            return reportPath;
        }



        static void DisplaySummary(List<FileAnalysisResult> results)
        {
            var successfulResults = results.Where(r => string.IsNullOrEmpty(r.Error)).ToList();
            var errorResults = results.Where(r => !string.IsNullOrEmpty(r.Error)).ToList();

            if (successfulResults.Any())
            {
                var totalLines = successfulResults.Sum(r => r.LineCount);
                var totalWords = successfulResults.Sum(r => r.WordCount);

                Console.WriteLine($"\nTotal de linhas: {totalLines:N0}");
                Console.WriteLine($"Total de palavras: {totalWords:N0}");
            }

            if (errorResults.Any())
            {
                Console.WriteLine($"\nArquivos com erro: {errorResults.Count}");
            }
        }
    }

    public class FileAnalysisResult
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int LineCount { get; set; }
        public int WordCount { get; set; }
        public int CharCount { get; set; }
        public long FileSizeBytes { get; set; }
        public DateTime AnalysisTime { get; set; }
        public string? Error { get; set; }
    }
}
