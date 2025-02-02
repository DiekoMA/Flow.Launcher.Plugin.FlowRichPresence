global using System;
global using System.Collections.Generic;
global using Flow.Launcher.Plugin;
global using System.Collections.ObjectModel;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;
global using DiscordRPC;
global using Flow.Launcher.Plugin.FlowRichPresence.Models;
global using System.Windows.Controls;
global using Flow.Launcher.Plugin.FlowRichPresence.ViewModels;
using Flow.Launcher.Plugin.FlowRichPresence.Views;

namespace Flow.Launcher.Plugin.FlowRichPresence
{
    public class FlowRichPresence : IPlugin, ISettingProvider
    {
        private static SettingsViewModel _viewModel;
        private PluginSettings _settings;
        private DiscordRpcClient client;
        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<PluginSettings>();
            _viewModel = new SettingsViewModel(_context, _settings);
            if (_settings.Profiles == null)
            {
                _settings.Profiles = new ObservableCollection<RPCProfile>();
            }
        }

        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();

            if (query.Search.Length < 2)
            {
                results.Add(new Result()
                {
                    Title = "Start",
                    SubTitle = "Select a profile to show off on discord.",
                    IcoPath = "Assets//PlayIcon.png"
                });
                
                results.Add(new Result()
                {
                    Title = "Stop",
                    SubTitle = "Select a profile to show off on discord.",
                    IcoPath = "Assets//StopIcon.png"
                });
            }

            switch (query.Search.ToLower())
            {
                case "start":
                    foreach (var profile in _settings.Profiles)
                    {
                        results.Add(new Result()
                        {
                            Title = profile.Title,
                            SubTitle = profile.ClientID,
                            IcoPath = "Assets//PluginLogo.png",
                            AsyncAction = async c =>
                            {
                                try
                                {
                                    client = new DiscordRpcClient(profile.ClientID);
                                    client.Initialize();
                                    profile.Presence.Assets = new Assets();
                                    client.ClearPresence();
                                    client.SetPresence(profile.Presence);   
                                }
                                catch (Exception e)
                                {
                                    _context.API.ShowMsgError(e.Message);
                                }
                                return true;
                            }
                        });
                    }
                    break;
                case "stop":
                    results.Add(new Result()
                    {
                        Title = $"Current Running Presence {client.CurrentPresence.State}",
                        SubTitle = "Running",
                        AsyncAction = async c =>
                        {
                            client.ClearPresence();
                            client.Dispose();
                            return true;
                        }
                    });
                    break;
            }
            return results;
        }

        public Control CreateSettingPanel()
        {
            return new SettingsView(_viewModel);
        }
    }
}