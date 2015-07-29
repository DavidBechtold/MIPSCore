using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MIPSCoreUI.BaseObjects
{
    public class CircuitObject : UserControl, ICircuitObject
    {
        //The lines being connected to the input
        private List<LineGeometry> startLines;
        private List<LineGeometry> endLines;

        public CircuitObject()
        {
            //Set the events for the object
            this.MouseLeftButtonDown += mouseLeftButtonDown;

            //Initialize the lists
            startLines = new List<LineGeometry>();
            endLines = new List<LineGeometry>();
        }

        public virtual void mouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        public void addStartLine(LineGeometry line)
        {
            startLines.Add(line);
        }

        public void addEndLine(LineGeometry line)
        {
            endLines.Add(line);
        }
    }
}
