// -----------------------------------------------------------------------
//  <copyright file="Main.java" company="Global Graphics Software Ltd">
//      Copyright (c) 2020 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

package com.valkyrie;

import com.globalgraphics.JawsMako.jawsmakoIF.*;

public class Main
{
    public static void main(String[] args)
    {
        var mako = IJawsMako.create();
        IJawsMako.enableAllFeatures(mako);

        var assembly = IPDFInput.create(mako).open("..\\..\\Test Files\\statement_sample_mako.pdf");
        var content = assembly.getDocument().getPage().getContent();

        var renderer = IJawsRenderer.create(mako);
        var image = renderer.renderAntiAliased(content);

        IDOMPNGImage.encode(mako, image, IOutputStream.createToFile(mako.getFactory(), "out.png"));
    }
}
