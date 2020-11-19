using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace CollisionDemo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly Random _random = new Random();
        private ObservableCollection<Ball> _balls;

        public ObservableCollection<Ball> Balls
        {
            get => _balls;
            set => SetProperty(ref _balls, value);
        }

        public MainWindowViewModel()
        {
            var balls = Enumerable.Range(0, 20).Select(i =>
            {
                var w = 15 + 15 * _random.NextDouble();
                return new Ball
                {
                    Mass = Math.Sqrt(w),
                    Position = new Point(100 + _random.NextDouble() * 300, 100 + _random.NextDouble() * 300),
                    Radius = w,
                    Speed = new Vector(_random.NextDouble() * 500 - 100, _random.NextDouble() * 500 - 100)
                };
            });
            Balls = new ObservableCollection<Ball>(balls);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
