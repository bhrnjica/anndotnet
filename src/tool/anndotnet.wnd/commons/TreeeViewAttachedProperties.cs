using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace anndotnet.wnd.Behaviours
{
    /// <summary>
    /// Attached properties
    /// </summary>
    public class TreeItemMouseLeftButtonDblClick
    {
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
            typeof(ICommand),
            typeof(TreeItemMouseLeftButtonDblClick),
            new UIPropertyMetadata(CommandChanged));

        public static DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
                                                typeof(object),
                                                typeof(TreeItemMouseLeftButtonDblClick),
                                                new UIPropertyMetadata(null));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }
        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Control control = target as Control;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.MouseDoubleClick += MouseDoubleClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.MouseDoubleClick -= MouseDoubleClick;
                }
            }
        }

        private static void MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            ICommand command = (ICommand)control.GetValue(CommandProperty);
            object commandParameter = control.GetValue(CommandParameterProperty);
            command.Execute(commandParameter);
            e.Handled = true;
        }


    }

    public class IsEditingBehaviour
    {
        public static bool GetIsEditing(TreeViewItem itm)
        {
            return (bool)itm.GetValue(IsEditingProperty);
        }

        public static void SetIsEditing(TreeViewItem itm, bool value)
        {
            itm.SetValue(IsEditingProperty, value);
        }

        public static readonly DependencyProperty IsEditingProperty =
          DependencyProperty.RegisterAttached("IsEditing", typeof(bool), typeof(IsEditingBehaviour),
                                            new UIPropertyMetadata(false));
    }
}
