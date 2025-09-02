# Nome: João Victor dos Santos Morais
# RM: 550453
## Turma - 3ESR

# FileAnalyzer - Processador Assíncrono de Arquivos de Texto

Aplicação Console em C# (.NET 8) que processa arquivos de texto de forma assíncrona, contando linhas e palavras, e gerando um relatório consolidado.

## Funcionalidades

- Seleção de diretório pelo usuário
- Busca automática de arquivos `.txt`
- Processamento assíncrono e paralelo
- Contagem de linhas e palavras
- Relatório consolidado em `./export/relatorio.txt`

## Requisitos

- .NET 8.0 SDK

## Como Usar

1. **Compilar:**
   ```bash
   dotnet build
   ```

2. **Executar:**
   ```bash
   dotnet run
   ```

3. **Seguir as instruções na tela**

## Formato do Relatório

O relatório é salvo em `./export/relatorio.txt` com o formato:
```
Arquivo - Linhas - Palavras
arquivo1.txt - 17 linhas - 51 palavras
arquivo2.txt - 25 linhas - 120 palavras
```

## Estrutura do Projeto

```
FileAnalyzer/
├── FileAnalyzer.csproj    # Projeto .NET 8
└── Program.cs             # Código fonte principal
```

## Tecnologias

- C# (.NET 8)
- async/await para processamento paralelo
- Manipulação de arquivos
