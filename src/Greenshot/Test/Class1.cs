using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Greenshot.Editor.Forms;
using NUnit.Framework;

namespace Greenshot.Test
{
    class Class1
    {
        [Test]
        public void Test1()
        {
            var x = new KeyEventArgs(Keys.Z);
            //ImageEditorForm.ImageEditorFormKeyDown(null, x);
            //SendKeys.Send("z");
        }
    }
}
