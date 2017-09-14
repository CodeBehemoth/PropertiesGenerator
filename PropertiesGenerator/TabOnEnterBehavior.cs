using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace PropertiesGenerator
{
    public abstract class OnEnterBehaviorBase : Behavior<Control>
    {
        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                doAction();
            }
        }
        protected abstract void doAction();

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }
    }


    public class TabOnEnterBehavior : OnEnterBehaviorBase
    {
        protected override void doAction()
        {
            var request = new TraversalRequest(FocusNavigationDirection.Next);
            request.Wrapped = true;
            AssociatedObject.MoveFocus(request);
        }
    }
}
