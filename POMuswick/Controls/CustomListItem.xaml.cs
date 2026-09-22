using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FFImageLoading.Maui;

namespace POMuswick.Controls
{
    public partial class CustomListItem : ContentView
    {
        public CustomListItem()
        {
            InitializeComponent();
        }

        #region ImageViewCommand

        public static readonly BindableProperty ImageViewCommandProperty =
            BindableProperty.Create(
                nameof(ImageViewCommand),
                typeof(ICommand),
                typeof(CustomListItem));

        public ICommand? ImageViewCommand
        {
            get => (ICommand?)GetValue(ImageViewCommandProperty);
            set => SetValue(ImageViewCommandProperty, value);
        }

        #endregion

        #region PlusCommand

        public static readonly BindableProperty PlusCommandProperty =
            BindableProperty.Create(
                nameof(PlusCommand),
                typeof(ICommand),
                typeof(CustomListItem));

        public ICommand? PlusCommand
        {
            get => (ICommand?)GetValue(PlusCommandProperty);
            set => SetValue(PlusCommandProperty, value);
        }

        #endregion

        #region MinusCommand

        public static readonly BindableProperty MinusCommandProperty =
            BindableProperty.Create(
                nameof(MinusCommand),
                typeof(ICommand),
                typeof(CustomListItem));

        public ICommand? MinusCommand
        {
            get => (ICommand?)GetValue(MinusCommandProperty);
            set => SetValue(MinusCommandProperty, value);
        }

        #endregion

        #region CommandParameter

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(
                nameof(CommandParameter),
                typeof(object),
                typeof(CustomListItem));

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        #endregion

        // [RelayCommand]
        // public async Task IncreaseQtyAsync(OrderDetail item)
        // {
        //     PlusCommand?.Execute(this);
        // }
        // [RelayCommand]
        // public async Task DecreaseQtyAsync(OrderDetail item)
        // {
        //     MinusCommand?.Execute(this);
        // }
    }
}