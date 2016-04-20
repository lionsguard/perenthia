using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Radiance.Markup;

namespace Perenthia.Controls
{
    public partial class ActorDetails : UserControl
    {
        public RdlActor Actor
        {
            get { return (RdlActor)GetValue(ActorProperty); }
            set { SetValue(ActorProperty, value); }
        }
        public static readonly DependencyProperty ActorProperty = DependencyProperty.Register("Actor", typeof(RdlActor), typeof(ActorDetails), new PropertyMetadata(null, new PropertyChangedCallback(ActorDetails.OnActorPropertyChanged)));
        private static void OnActorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as ActorDetails).Refresh();
        }
            
        public ActorDetails()
        {
            InitializeComponent();
        }

        private void Refresh()
        {
            if (this.Actor != null)
            {
            }
        }
    }
}
