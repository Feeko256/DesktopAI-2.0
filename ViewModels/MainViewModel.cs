using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DesktopAI.Core;
using DesktopAI.Models;
using MaterialDesignThemes.Wpf;

namespace DesktopAI.ViewModels;

public class MainViewModel : BaseViewModel
{
    
    string apiKey = "";
    string endpoint = "https://api.openai.com/v1/chat/completions";
    ObservableCollection<Message> messages;
    HttpClient httpClient;
    private string _content;
    private RelayCommand sendMessageCommand;
    private bool isIndeterminate;
    public ObservableCollection<DialogModel> Dialog { get; set; }
    private float columntWidth;
    private RelayCommand authorizationCommand;
    public ObservableCollection<ObservableCollection<Message>> DialogList { get; set; }
    private string totalTokensUsage;

    public string TotalTokenUsage
    {
        get => totalTokensUsage;
        set
        {
            totalTokensUsage = value;
            OnPropertyChanged();
        }
    }

    public string ApiKey
    {
        get => apiKey;
        set
        {
            apiKey = value;
            OnPropertyChanged();

        }
    }

    public float ColumnWidth
    {
        get => columntWidth;
        set
        {
            columntWidth = value;
            OnPropertyChanged();
        }
    }

    public string Content
    {
        get => _content;
        set
        {

            _content = value;
            //  if (_content.Length>0)
            //    ColumnWidth = 80;
            if (Content.Length > 0)
            {
                Task.Run(ShowColumn);
            }

            Task.Run(HideColumn);
            OnPropertyChanged();
        }
    }


    public bool IsIndeterminate
    {
        get => isIndeterminate;
        set
        {
            isIndeterminate = value;
            OnPropertyChanged();
        }
    }

    public async Task ShowColumn()
    {
        
            if (ColumnWidth == 0)
                for (int i = 0; i <= 11; i++)
                {
                    ColumnWidth += i;
                    await Task.Delay(10); // add a delay to make the animation smooth
                }
        
    }
    public async Task HideColumn()
    {
        if (Content.Length == 0)
        {
                for (int i = 0; i <= 11; i++)
                {
                    ColumnWidth -= i;
                    await Task.Delay(10); // add a delay to make the animation smooth
                }
            
        
        }
    }

    public RelayCommand AuthorizationCommand
    {
        get
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            return authorizationCommand ??= new RelayCommand(async obj =>
            {
              //  var a = ApiKey;
                key = new Key("");
                key._Key = ApiKey;
                SaveLoad.Save(key);
                //MessageBox.Show(key._Key);
                TotalTokenUsage = "0";
                messages = new ObservableCollection<Message>();
               
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
                DialogList.Add(messages);
       
             
                Dialog.Add(new DialogModel
                {
                    Content = "Welcome to th new chat!",
                    HorizontalAlignment = HorizontalAlignment.Center
                });
                IsIndeterminate = false;
                
            }, o => ApiKey != "");
        }
    }

    public RelayCommand SendMessageCommand
    {
        get
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            return sendMessageCommand ??= new RelayCommand(async obj =>
            {
                try
                {
                    IsIndeterminate = true;
                    var message = new Message() { Role = "user", Content = Content };
                    messages.Add(message);
                    Dialog.Add(new DialogModel
                    {
                        Content = Content, HorizontalAlignment = HorizontalAlignment.Right, PathToLogo = "../UserLogo.png", BackgroundColor = new SolidColorBrush(Color.FromRgb(88, 140, 109))
                    });
                    Content = "";
                    var requestData = new Request()
                    {
                        ModelId = "gpt-3.5-turbo",
                        Messages = messages
                    };
                    using var response = await httpClient.PostAsJsonAsync(endpoint, requestData);

                    if (!response.IsSuccessStatusCode)
                    {
                        Dialog.Add(new DialogModel
                        {
                            Content = $"{(int)response.StatusCode} {response.StatusCode}",
                            HorizontalAlignment = HorizontalAlignment.Center
                        });
                    }

                    ResponseData? responseData = await response.Content.ReadFromJsonAsync<ResponseData>();

                    var choices = responseData?.Choices ?? new List<Choice>();
                    if (choices.Count == 0)
                    {
                        Dialog.Add(new DialogModel
                        {
                            Content = "No choices were returned by the API",
                            HorizontalAlignment = HorizontalAlignment.Center
                        });
                    }

                    var choice = choices[0];
                    var responseMessage = choice.Message;
                    messages.Add(responseMessage);
                    Dialog.Add(new DialogModel
                    {
                        Content = responseMessage.Content, HorizontalAlignment = HorizontalAlignment.Left,
                        PathToLogo = "../GPTLogo.png", BackgroundColor = new SolidColorBrush(Colors.DimGray)
                    });
                    IsIndeterminate = false;
                    TotalTokenUsage = responseData.Usage.TotalTokens.ToString();
                }
                catch (Exception e)
                {
                    IsIndeterminate = false;
                    messages = new ObservableCollection<Message>();
                    DialogList.Add(messages);
                    Dialog.Add(new DialogModel
                    {
                        Content = "u broke bot :< lets start new chat",
                        HorizontalAlignment = HorizontalAlignment.Center
                    });
                }
                
            }, obj=> messages!=null && (!string.IsNullOrWhiteSpace(Content)));
        }
    }

    private Key key;

    public MainViewModel()
    {
        key = SaveLoad.Load();
        Dialog = new ObservableCollection<DialogModel>();
        DialogList = new ObservableCollection<ObservableCollection<Message>>();
        ApiKey = key._Key;
        IsIndeterminate = false;
        ColumnWidth = 0;
        TotalTokenUsage = "0";
    }
}