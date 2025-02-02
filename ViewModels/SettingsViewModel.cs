namespace Flow.Launcher.Plugin.FlowRichPresence.ViewModels;

public class SettingsViewModel : ObservableObject
{
    private ObservableCollection<RPCProfile> _profiles;

    public ObservableCollection<RPCProfile> Profiles
    {
        get => _settings.Profiles;
        set
        {
            _profiles = value;
            SetProperty(ref _profiles, value);
            OnPropertyChanged();
        }
    }
    private RPCProfile _currentProfile;

    public RPCProfile CurrentProfile
    {
        get => _currentProfile;
        set
        {
            _currentProfile = value;
            SetProperty(ref _currentProfile, value);
            OnPropertyChanged();
            _context.API.SavePluginSettings();
        }
    }
    
    private PluginInitContext _context;
    private PluginSettings _settings;
    public IRelayCommand CreateNewProfileCommand { get; }
    public IRelayCommand RemoveCurrentProfileCommand { get; }

    public SettingsViewModel(PluginInitContext context, PluginSettings settings)
    {
        this._context = context;
        this._settings = settings;
        CreateNewProfileCommand = new RelayCommand(CreateNewProfile);
        RemoveCurrentProfileCommand = new RelayCommand(RemoveCurrentProfile);
    }

    private void CreateNewProfile()
    {
        if (_settings.Profiles != null)
        {
            var newProfile = new RPCProfile()
            {
                ClientID = string.Empty,
                Title = "New Profile",
                Presence = new RichPresence()
                {
                    State = "Flow RPC",
                    Details = "Created and Running from flow launcher",
                    Assets = new Assets()
                    {
                        LargeImageKey = "https://t2.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=http://flowlauncher.com&size=128",
                        LargeImageText = "Flow RPC",
                        SmallImageKey = "https://t2.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=http://flowlauncher.com&size=128",
                        SmallImageText = "Flow RPC",
                    },
                },
            };
            _settings.Profiles.Add(newProfile);
            CurrentProfile = newProfile;
            _context.API.SavePluginSettings();
        }
    }

    private void RemoveCurrentProfile()
    {
        CurrentProfile = null;
        _settings.Profiles.Remove(CurrentProfile);
        _context.API.SavePluginSettings();
    }
}