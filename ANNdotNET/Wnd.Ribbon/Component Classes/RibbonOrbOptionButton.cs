// *********************************
// Message from Original Author:
//
// 2008 Jose Menendez Poo
// Please give me credit if you use this code. It's all I ask.
// Contact me for more info: menendezpoo@gmail.com
// *********************************
//
// Original project from http://ribbon.codeplex.com/
// Continue to support and maintain by http://officeribbon.codeplex.com/

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace System.Windows.Forms
{
    public class RibbonOrbOptionButton
        : RibbonButton
    {
        #region Ctors

        public RibbonOrbOptionButton()
            : base()
        {

        }

        public RibbonOrbOptionButton(string text)
            : this()
        {
            Text = text;
        }

        #endregion

        #region Props
        public override System.Drawing.Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                base.Image = value;

                SmallImage = value;
            }
        }

        [Browsable(false)]
        public override System.Drawing.Image SmallImage
        {
            get
            {
                return base.SmallImage;
            }
            set
            {
                base.SmallImage = value;
            }
        }

        #endregion
    }
}
