﻿using System.Collections.ObjectModel;
using CustomServiceTestUtil.Mvvm;

namespace CustomServiceTestUtil.ViewModels
{
    internal class ViewModelBase : BindableBase
    {
        private static readonly ObservableCollection<MenuItem> AppMenu = new ObservableCollection<MenuItem>();
        private static readonly ObservableCollection<MenuItem> AppOptionsMenu = new ObservableCollection<MenuItem>();

        public ViewModelBase()
        {
        }

        public ObservableCollection<MenuItem> Menu => AppMenu;

        public ObservableCollection<MenuItem> OptionsMenu => AppOptionsMenu;
    }
}