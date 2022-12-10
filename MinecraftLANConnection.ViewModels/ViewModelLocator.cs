using System;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace MinecraftLANConnection.ViewModels
{
    public sealed class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public static MainViewModel MainViewModel => SimpleIoc.Default.GetInstance<MainViewModel>();
    }
}

