using System.Collections.Generic;

namespace CoreApp.Application.ViewModels
{
    public class TreeViewModel<T>
    {
        public T Id { get; set; }
        public string Text { get; set; }
        public List<TreeViewModel<T>> Children { get; set; }
    }
}
