using Pollux.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.ViewModel
{
    public class CheckableModel<T> : BindableBase
    {
        public CheckableModel()
        {
        }
        public CheckableModel(T model)
        {
            Model = model;
        }
        private bool _IsChecked = false;
        public bool  IsChecked
        {
            get { return _IsChecked; }
            set
            {
                SetProperty(ref _IsChecked, value);
            }
        }
        
        private T _Model;
        public T Model
        {
            get { return _Model; }
            set
            {
                SetProperty<T>(ref _Model, value);
            }
        }
        public static explicit operator CheckableModel<T>(T model)
        {
            return new CheckableModel<T>(model);
        }

        public static implicit operator T(CheckableModel<T> item) 
        {
            return item.Model;
        }
        
    }
    public class CheckableCollection<T> : ObservableCollection<CheckableModel<T>>
    {
        public CheckableCollection()
            : base()
        {
        }
        public CheckableCollection(IEnumerable<CheckableModel<T>> collection) : base(collection)
        {
        }

        public CheckableCollection(IEnumerable<T> collection)
            : base(collection.Select(i => new CheckableModel<T> { IsChecked = false, Model = i }))
        {
        }

        public void Select(IEnumerable<T> items)
        {
            foreach(var item in Items)
            {
                item.IsChecked = items.Contains(item.Model);
            }

        }
        public IEnumerable<T> SelectedItems 
        {
            get
            {
                return this.Items.Where(i => i.IsChecked).Select(i=>i.Model);
            }
        }
        public void Remove()
        {
            foreach (var item in Items.Where(i => i.IsChecked == true))
            {
                Remove(item);
            }
        }
    }
}
