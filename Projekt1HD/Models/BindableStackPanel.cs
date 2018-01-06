using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Projekt1HD.Models
{
    public class BindableStackPanel : BindableBase
    {

        private StackPanel _stkPanel;
        public StackPanel StkPanel
        {
            get { return _stkPanel; }
            set { SetProperty(ref _stkPanel, value); }
        }
    }
}