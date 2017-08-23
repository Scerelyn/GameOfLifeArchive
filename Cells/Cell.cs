using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameOfLife.Cells
{
    public class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// The Brush the cells uses when IsAlive is true
        /// </summary>
        public readonly static Brush TrueBrush = new SolidColorBrush()
        {
            Color = new Color()
            {
                R = 25,
                G = 52,
                B = 65,
                A = 255,
            }
        };
        /// <summary>
        /// The Brush to use when the cell has an IsAlive value of false
        /// </summary>
        public readonly static Brush FalseBrush = new SolidColorBrush()
        {
            Color = new Color()
            {
                R = 145,
                G = 170,
                B = 157,
                A = 255,
            }
        };

        private bool isAlive = false;
        public bool IsAlive
        {
            get
            {
                return isAlive;
            }
            set
            {
                isAlive = value;
                FieldChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void FieldChanged(string field = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(field));
            }
        }
    }
}
