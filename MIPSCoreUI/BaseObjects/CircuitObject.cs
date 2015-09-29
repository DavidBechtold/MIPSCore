using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MIPSCoreUI.BaseObjects
{
    public class CircuitObject : UserControl
    {
        //The lines being connected to the input
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<LineGeometry> startLines;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<LineGeometry> endLines;

        public CircuitObject()
        {
            //Set the events for the object
            MouseLeftButtonDown += MouseLeftButtonDownNew;

            //Initialize the lists
            startLines = new List<LineGeometry>();
            endLines = new List<LineGeometry>();
        }

        public virtual void MouseLeftButtonDownNew(object sender, MouseButtonEventArgs e)
        {
        }

        public void AddStartLine(LineGeometry line)
        {
            startLines.Add(line);
        }

        public void AddEndLine(LineGeometry line)
        {
            endLines.Add(line);
        }
    }
}
