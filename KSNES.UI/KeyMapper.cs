﻿using Configuration = System.Configuration.Configuration;

namespace KSNES.GUI;

public class KeyMapper : IKeyMapper
{
    private Dictionary<Keys, SNESButton> _dict = [];
    private Dictionary<SNESButton, Keys> _reverseDict = [];

    private readonly Dictionary<string, Keys> _settingToKeyMapping = new()
    {
        {"KeyA", Keys.A},
        {"KeyB", Keys.B},
        {"KeyY", Keys.Y},
        {"KeyX", Keys.X},
        {"KeyL", Keys.L},
        {"KeyR", Keys.R},
        {"KeyStart", Keys.Enter},
        {"KeySelect", Keys.Tab},
        {"KeyLeft", Keys.Left},
        {"KeyRight", Keys.Right},
        {"KeyUp", Keys.Up},
        {"KeyDown", Keys.Down}
    };

    private readonly Dictionary<string, SNESButton> _settingToButtonMapping = new()
    {
        {"KeyA", SNESButton.A},
        {"KeyB", SNESButton.B},
        {"KeyY", SNESButton.Y},
        {"KeyX", SNESButton.X},
        {"KeyL", SNESButton.L},
        {"KeyR", SNESButton.R},
        {"KeyStart", SNESButton.Start},
        {"KeySelect", SNESButton.Sel},
        {"KeyLeft", SNESButton.Left},
        {"KeyRight", SNESButton.Right},
        {"KeyUp", SNESButton.Up},
        {"KeyDown", SNESButton.Down}
    };

    public KeyMapper()
    {
        foreach (KeyValuePair<string, Keys> kvp in _settingToKeyMapping)
        {
            var temp = GetKeysFromSettings(kvp.Key) ?? kvp.Value;
            _dict.Add(temp, _settingToButtonMapping[kvp.Key]);
            _reverseDict.Add(_settingToButtonMapping[kvp.Key], temp);
        }
    }

    public SNESButton? Map(Keys key)
    {
        if (_dict.TryGetValue(key, out SNESButton value))
        {
            return value;
        }
        return null;
    }

    public Keys? ReverseMap(SNESButton button)
    {
        if (_reverseDict.TryGetValue(button, out Keys value))
        {
            return value;
        }
        return null;
    }

    public void Clear()
    {
        _dict = [];
        _reverseDict = [];
}

    public void ChangeMap(Keys key, SNESButton button)
    {
        _dict.Add(key, button);
        _reverseDict.Add(button, key);
    }

    public void SaveChanges()
    {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        foreach (KeyValuePair<string, SNESButton> kvp in _settingToButtonMapping)
        {
            string toSave = ((int)_reverseDict[kvp.Value]).ToString();
            if (config.AppSettings.Settings[kvp.Key] == null)
            {
                config.AppSettings.Settings.Add(kvp.Key, toSave);
            }
            else
            {
                config.AppSettings.Settings[kvp.Key].Value = toSave;
            }
        }
        config.Save(ConfigurationSaveMode.Minimal);
        ConfigurationManager.RefreshSection("appSettings");
    }

    private static Keys? GetKeysFromSettings(string key)
    {
        string? value = ConfigurationManager.AppSettings[key];
        if (value == null)
        {
            return null;
        }
        if (int.TryParse(value, out int returnValue))
        {
            return (Keys) returnValue;
        }
        return null;
    }
}