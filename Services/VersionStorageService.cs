using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DiscordBotUpdate.Services
{
    public class VersionStorageService
    {
        private readonly string _filePath = "versions.json";
        private readonly ILogger<VersionStorageService> _logger;
        private Dictionary<string, string> _versions;

        public VersionStorageService(ILogger<VersionStorageService> logger)
        {
            _logger = logger;
            LoadVersions();
        }

        private void LoadVersions()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _logger?.LogInformation("Arquivo {FilePath} não encontrado. Criando novo arquivo.", _filePath);
                    _versions = new Dictionary<string, string>();
                    SaveVersions();
                }
                else
                {
                    var json = File.ReadAllText(_filePath);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        _logger?.LogWarning("Arquivo {FilePath} está vazio. Inicializando dicionário vazio.", _filePath);
                        _versions = new Dictionary<string, string>();
                        SaveVersions();
                    }
                    else
                    {
                        _versions = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                        _logger?.LogInformation("Arquivo {FilePath} carregado com {Count} entradas.", _filePath, _versions.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao carregar o arquivo {FilePath}. Inicializando dicionário vazio.", _filePath);
                _versions = new Dictionary<string, string>();
                SaveVersions();
            }
        }

        private void SaveVersions()
        {
            try
            {
                var json = JsonSerializer.Serialize(_versions, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
                _logger?.LogInformation("Arquivo {FilePath} salvo com sucesso.", _filePath);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao salvar o arquivo {FilePath}.", _filePath);
            }
        }

        public string GetVersion(string client)
        {
            if (string.IsNullOrWhiteSpace(client))
                return null;
            return _versions.TryGetValue(client.ToLower(), out var version) ? version : null;
        }

        public void UpdateVersion(string client, string version)
        {
            if (string.IsNullOrWhiteSpace(client) || string.IsNullOrWhiteSpace(version))
            {
                _logger?.LogWarning("Tentativa de atualizar versão com cliente ou versão inválidos: client={Client}, version={Version}", client, version);
                return;
            }

            // Opcional: Validar formato da versão (ex.: x.y.z)
            if (!System.Text.RegularExpressions.Regex.IsMatch(version, @"^\d+\.\d+\.\d+$"))
            {
                _logger?.LogWarning("Formato de versão inválido: {Version}", version);
                return;
            }

            _versions[client.ToLower()] = version;
            SaveVersions();
        }

        public IReadOnlyDictionary<string, string> GetAllVersions()
        {
            return _versions.AsReadOnly();
        }
    }
}