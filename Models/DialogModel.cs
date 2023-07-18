using System.Windows;
using System.Windows.Media;
using DesktopAI.Core;

namespace DesktopAI.Models;

public class DialogModel : BaseViewModel
{
    private string content;
    private HorizontalAlignment horizontalAlignment;
    private string pathToLogo;
    private Brush backgroundColor;

    public Brush BackgroundColor
    {
        get => backgroundColor;
        set
        {
            backgroundColor = value;
            OnPropertyChanged();
        }
    }

    public string PathToLogo
    {
        get => pathToLogo;
        set
        {
            pathToLogo = value;
            OnPropertyChanged();
        }
    }

    public string Content
    {
        get => content;
        set
        {
            content = value;
            OnPropertyChanged();
        }
    }
    public HorizontalAlignment HorizontalAlignment
    {
        get => horizontalAlignment;
        set
        {
            horizontalAlignment = value;
            OnPropertyChanged();
        }
    }
    
}