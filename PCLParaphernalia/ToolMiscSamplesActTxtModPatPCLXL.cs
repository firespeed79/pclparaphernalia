﻿using System;
using System.IO;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides PCL XL support for the Text and Background element
    /// of the Text Modification action of the MiscSamples tool.
    /// 
    /// © Chris Hutchinson 2014
    /// 
    /// </summary>

    static class ToolMiscSamplesActTxtModPatPCLXL
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const String _formName           = "MiscTxtModPatForm";

        const Int32 _symSet_19U          = 629;
        const UInt16 _unitsPerInch       = PCLXLWriter._sessionUPI;

        const Int16 _pageOriginX         = (_unitsPerInch * 1);
        const Int16 _pageOriginY         = (_unitsPerInch * 1);
        const Int16 _incInch             = (_unitsPerInch * 1);
        const Int16 _lineInc             = (_unitsPerInch * 5) / 6;

        const Int16 _posXDesc            = _pageOriginX;
        const Int16 _posXData            = _pageOriginX + (2 * _incInch);

        const Int16 _posYHddr            = _pageOriginY;
        const Int16 _posYDesc            = _pageOriginY + (2 * _incInch);
        const Int16 _posYData            = _pageOriginY + (2 * _incInch);

        const Int16 _patternId_DarkGrey  = 301;
        const Int16 _patternId_LightGrey = 302;

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Static variables.                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        static Int16 _fontIndexArial   = PCLFonts.getIndexForName ("Arial");
        static Int16 _fontIndexCourier = PCLFonts.getIndexForName ("Courier");

        static String _fontNameArial    =
            PCLFonts.getPCLXLName (_fontIndexArial,
                                  PCLFonts.eVariant.Regular);
        static String _fontNameCourier  = 
            PCLFonts.getPCLXLName (_fontIndexCourier,
                                  PCLFonts.eVariant.Regular);
        static String _fontNameCourierBold  = 
            PCLFonts.getPCLXLName (_fontIndexCourier,
                                  PCLFonts.eVariant.Bold);

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Generate test data.                                                //
        //                                                                    //
        // Some sequences are built up as (Unicode) strings, then converted   //
        // to byte arrays before writing out - this works OK because all the  //
        // characters we're using are within the ASCII range (0x00-0x7f) and  //
        // are hence represented using a single byte in the UTF-8 encoding.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void generateJob(BinaryWriter prnWriter,
                                       Int32        indxPaperSize,
                                       Int32        indxPaperType,
                                       Int32        indxOrientation,
                                       Boolean      formAsMacro)
        {
            generateJobHeader(prnWriter);

            if (formAsMacro)
                generateOverlay(prnWriter, true,
                                indxPaperSize, indxOrientation);

            generatePage(prnWriter,
                         indxPaperSize,
                         indxPaperType,
                         indxOrientation,
                         formAsMacro);
 
            generateJobTrailer(prnWriter, formAsMacro);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b H e a d e r                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write stream initialisation sequences to output file.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobHeader(BinaryWriter prnWriter)
        {
            PCLXLWriter.stdJobHeader(prnWriter, "");
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e J o b T r a i l e r                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write tray map termination sequences to output file.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateJobTrailer(BinaryWriter prnWriter,
                                               Boolean      formAsMacro)
        {
            PCLXLWriter.stdJobTrailer(prnWriter, formAsMacro, _formName);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e O v e r l a y                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write background data sequences to output file.                    //
        // Optionally top and tail these with macro (user-defined stream)     //
        // definition sequences.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generateOverlay(BinaryWriter prnWriter,
                                            Boolean      formAsMacro,
                                            Int32        indxPaperSize,
                                            Int32        indxOrientation)
        {
            const Int32 lenBuf = 1024;

            Byte[] buffer = new Byte[lenBuf];

            Int16 ptSize;

            Int32 indBuf;

            Int16 posX,
                  posY;

            UInt16 boxX1,
                   boxX2,
                   boxY1,
                   boxY2;

            Int16 rectX,
                  rectY,
                  rectHeight,
                  rectWidth;

            Byte stroke = 1;

            //----------------------------------------------------------------//

            indBuf = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Header                                                         //
            // Parts of overlay use different brush and/or pen definitions,   //
            // so enclosed in a GS block.                                     //
            //                                                                //
            //----------------------------------------------------------------//

            if (formAsMacro)
            {
                PCLXLWriter.streamHeader(prnWriter, true, _formName);
            }

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PushGS);

            //----------------------------------------------------------------//
            //                                                                //
            // Colour space, pen & brush definitions.                         //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUbyte (ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.ColorSpace,
                                     (Byte) PCLXLAttrEnums.eVal.eGray);

            PCLXLWriter.addAttrUbyte (ref buffer,
                                     ref indBuf,
                                     PCLXLAttributes.eTag.PaletteDepth,
                                     (Byte) PCLXLAttrEnums.eVal.e8Bit);

            PCLXLWriter.addAttrUbyteArray (ref buffer,
                                          ref indBuf,
                                          PCLXLAttributes.eTag.PaletteData,
                                          2,
                                          PCLXLWriter.monoPalette);

            PCLXLWriter.addOperator (ref buffer,
                                    ref indBuf,
                                    PCLXLOperators.eTag.SetColorSpace);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.NullBrush,
                               0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenSource);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.PenWidth,
                               stroke);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenWidth);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//
            //                                                                //
            // Box.                                                           //
            //                                                                //
            //----------------------------------------------------------------//

            boxX1 = _unitsPerInch / 2;  // half-inch left margin
            boxY1 = _unitsPerInch / 2;  // half-inch top-margin

            boxX2 = (UInt16) (PCLPaperSizes.getPaperWidth (
                                    indxPaperSize, _unitsPerInch,
                                    PCLOrientations.eAspect.Portrait) -
                              boxX1); 

            boxY2 = (UInt16) (PCLPaperSizes.getPaperLength (
                                    indxPaperSize, _unitsPerInch,
                                    PCLOrientations.eAspect.Portrait) -
                              boxY1); 

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.TxMode,
                               (Byte)PCLXLAttrEnums.eVal.eTransparent);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPatternTxMode);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.TxMode,
                               (Byte)PCLXLAttrEnums.eVal.eTransparent);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetSourceTxMode);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               100);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenSource);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.PenWidth,
                               5);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenWidth);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.NullBrush,
                               0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetBrushSource);
            
            PCLXLWriter.addAttrUint16XY(ref buffer,
                                  ref indBuf,
                                  PCLXLAttributes.eTag.EllipseDimension,
                                  100, 100);

            PCLXLWriter.addAttrUint16Box(ref buffer,
                                   ref indBuf,
                                   PCLXLAttributes.eTag.BoundingBox,
                                   boxX1, boxY1, boxX2, boxY2);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.RoundRectangle);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//
            //                                                                //
            // Text.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               100);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetBrushSource);

            PCLXLWriter.addAttrUbyte(ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.NullPen,
                               0);

            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.SetPenSource);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            ptSize  = 15;
            
            PCLXLWriter.font(prnWriter, formAsMacro, ptSize,
                             _symSet_19U, _fontNameCourierBold);

            posX = _posXDesc;
            posY = _posYHddr;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "PCL XL Text & Background:");

            ptSize  = 12;
            
            PCLXLWriter.font(prnWriter, formAsMacro, ptSize,
                             _symSet_19U, _fontNameCourier);

            posY = _posYDesc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                      "Black:");

            posY += _lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       "Shade = " + "Dark Gray:");

            posY += _lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                      "Shade = " + "Light Gray:");

            posY += _lineInc;

            PCLXLWriter.text(prnWriter, formAsMacro, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                      "White:");

            //----------------------------------------------------------------//
            //                                                                //
            // Background shading.                                            //
            //                                                                //
            //----------------------------------------------------------------//

            patternDefineDpi300 (prnWriter, formAsMacro);

            rectHeight = (Int16) ((_lineInc * 3) / 5);
            rectWidth = (Int16) ((_unitsPerInch * 9) / 10);

            posX = _posXData;
            posY = _posYData - (_lineInc / 2);

            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUbyte (ref buffer,
                               ref indBuf,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.addOperator (ref buffer,
                                     ref indBuf,
                                     PCLXLOperators.eTag.SetBrushSource);

            rectX = posX;
            rectY = posY;

            for (Int32 i = 0; i < 4; i++)
            {
                PCLXLWriter.addAttrUint16Box (ref buffer,
                                             ref indBuf,
                                             PCLXLAttributes.eTag.BoundingBox,
                                             (UInt16) rectX,
                                             (UInt16) rectY,
                                             (UInt16) (rectX + rectWidth),
                                             (UInt16) (rectY + rectHeight));

                PCLXLWriter.addOperator (ref buffer,
                                        ref indBuf,
                                        PCLXLOperators.eTag.Rectangle);

                rectY += _lineInc;
            }

            PCLXLWriter.writeStreamBlock (prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//

            PCLXLWriter.addAttrSint16 (ref buffer,
                                       ref indBuf,
                                       PCLXLAttributes.eTag.PatternSelectID,
                                       _patternId_DarkGrey);

            PCLXLWriter.addAttrSint16XY (ref buffer,
                                         ref indBuf,
                                         PCLXLAttributes.eTag.PatternOrigin,
                                         0, 0);

            PCLXLWriter.addOperator (ref buffer,
                                     ref indBuf,
                                     PCLXLOperators.eTag.SetBrushSource);

            rectX += rectWidth;
            rectY = posY;

            for (Int32 i = 0; i < 4; i++)
            {
                PCLXLWriter.addAttrUint16Box (ref buffer,
                                             ref indBuf,
                                             PCLXLAttributes.eTag.BoundingBox,
                                             (UInt16) rectX,
                                             (UInt16) rectY,
                                             (UInt16) (rectX + rectWidth),
                                             (UInt16) (rectY + rectHeight));

                PCLXLWriter.addOperator (ref buffer,
                                        ref indBuf,
                                        PCLXLOperators.eTag.Rectangle);

                rectY += _lineInc;
            }

            PCLXLWriter.writeStreamBlock (prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//

            PCLXLWriter.addAttrSint16 (ref buffer,
                                       ref indBuf,
                                       PCLXLAttributes.eTag.PatternSelectID,
                                       _patternId_LightGrey);

            PCLXLWriter.addAttrSint16XY (ref buffer,
                                         ref indBuf,
                                         PCLXLAttributes.eTag.PatternOrigin,
                                         0, 0);

            PCLXLWriter.addOperator (ref buffer,
                                     ref indBuf,
                                     PCLXLOperators.eTag.SetBrushSource);

            rectX += rectWidth;
            rectY = posY;

            for (Int32 i = 0; i < 4; i++)
            {
                PCLXLWriter.addAttrUint16Box (ref buffer,
                                             ref indBuf,
                                             PCLXLAttributes.eTag.BoundingBox,
                                             (UInt16) rectX,
                                             (UInt16) rectY,
                                             (UInt16) (rectX + rectWidth),
                                             (UInt16) (rectY + rectHeight));

                PCLXLWriter.addOperator (ref buffer,
                                        ref indBuf,
                                        PCLXLOperators.eTag.Rectangle);

                rectY += _lineInc;
            }

            PCLXLWriter.writeStreamBlock (prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            //----------------------------------------------------------------//
            //                                                                //
            // Overlay end.                                                   //
            //                                                                //
            //----------------------------------------------------------------//
            
            PCLXLWriter.addOperator(ref buffer,
                              ref indBuf,
                              PCLXLOperators.eTag.PopGS);

            PCLXLWriter.writeStreamBlock(prnWriter, formAsMacro,
                                   buffer, ref indBuf);

            if (formAsMacro)
            {
                PCLXLWriter.addOperator(ref buffer,
                                  ref indBuf,
                                  PCLXLOperators.eTag.EndStream);

                prnWriter.Write(buffer, 0, indBuf);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e n e r a t e P a g e                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write individual test data page sequences to output file.          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void generatePage(BinaryWriter prnWriter,
                                         Int32        indxPaperSize,
                                         Int32        indxPaperType,
                                         Int32        indxOrientation,
                                         Boolean      formAsMacro)
        {
            const Int32 sizeStd = 1024;

            Byte[] bufStd = new Byte[sizeStd];

            String sampleText = "000000000000000";

            Int16 posX,
                  posY;
            
            Int32 indStd;

            Int16 ptSize;

            indStd = 0;

            //----------------------------------------------------------------//

            if (indxOrientation < PCLOrientations.getCount())
            {
                PCLXLWriter.addAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.Orientation,
                                   PCLOrientations.getIdPCLXL(indxOrientation));
            }

            if (indxPaperSize < PCLPaperSizes.getCount())
            {
                PCLXLWriter.addAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.eTag.MediaSize,
                                   PCLPaperSizes.getIdPCLXL(indxPaperSize));
            }

            if ((indxPaperType < PCLPaperTypes.getCount()) &&
                (PCLPaperTypes.getType(indxPaperType) !=
                    PCLPaperTypes.eEntryType.NotSet))
            {
                PCLXLWriter.addAttrUbyteArray(ref bufStd,
                                        ref indStd,
                                        PCLXLAttributes.eTag.MediaType,
                                        PCLPaperTypes.getName(indxPaperType));
            }

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.SimplexPageMode,
                               (Byte)PCLXLAttrEnums.eVal.eSimplexFrontSide);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.BeginPage);

            PCLXLWriter.addAttrUint16XY(ref bufStd,
                                  ref indStd,
                                  PCLXLAttributes.eTag.PageOrigin,
                                  0, 0);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetPageOrigin);

            PCLXLWriter.addAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.ColorSpace,
                               (Byte)PCLXLAttrEnums.eVal.eGray);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetColorSpace);

            prnWriter.Write(bufStd, 0, indStd);
            indStd = 0;
            
            //----------------------------------------------------------------//

            if (formAsMacro)
            {
                PCLXLWriter.addAttrUbyteArray(ref bufStd,
                                        ref indStd,
                                        PCLXLAttributes.eTag.StreamName,
                                        _formName);

                PCLXLWriter.addOperator(ref bufStd,
                                  ref indStd,
                                  PCLXLOperators.eTag.ExecStream);

                prnWriter.Write(bufStd, 0, indStd);
                indStd = 0;
            }
            else
            {
                generateOverlay(prnWriter, false,
                                indxPaperSize, indxOrientation);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Text.                                                          //
            //                                                                //
            //----------------------------------------------------------------//

            ptSize = 34;

            PCLXLWriter.font (prnWriter, false, ptSize,
                             _symSet_19U, _fontNameArial);

            PCLXLWriter.addAttrUbyte (ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.NullPen,
                               0);

            PCLXLWriter.addOperator (ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetPenSource);

            //----------------------------------------------------------------//
            // Black                                                          //
            //----------------------------------------------------------------//

            posX = _posXData;
            posY = _posYData;

            PCLXLWriter.addAttrUbyte (ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               0);

            PCLXLWriter.addOperator (ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);
            
            prnWriter.Write (bufStd, 0, indStd);
            indStd = 0;

            PCLXLWriter.text (prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       sampleText);

            //----------------------------------------------------------------//
            // Shade 1                                                        //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLXLWriter.addAttrSint16 (ref bufStd,
                                       ref indStd,
                                       PCLXLAttributes.eTag.PatternSelectID,
                                       _patternId_DarkGrey);

            PCLXLWriter.addAttrSint16XY (ref bufStd,
                                         ref indStd,
                                         PCLXLAttributes.eTag.PatternOrigin,
                                         0, 0);

            PCLXLWriter.addOperator (ref bufStd,
                                     ref indStd,
                                     PCLXLOperators.eTag.SetBrushSource);

            prnWriter.Write (bufStd, 0, indStd);
            indStd = 0;

            PCLXLWriter.text (prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       sampleText);

            //----------------------------------------------------------------//
            // Shade 2                                                        //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLXLWriter.addAttrSint16 (ref bufStd,
                                       ref indStd,
                                       PCLXLAttributes.eTag.PatternSelectID,
                                       _patternId_LightGrey);

            PCLXLWriter.addAttrSint16XY (ref bufStd,
                                         ref indStd,
                                         PCLXLAttributes.eTag.PatternOrigin,
                                         0, 0);

            PCLXLWriter.addOperator (ref bufStd,
                                     ref indStd,
                                     PCLXLOperators.eTag.SetBrushSource);

            prnWriter.Write (bufStd, 0, indStd);
            indStd = 0;

            PCLXLWriter.text (prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       sampleText);

            //----------------------------------------------------------------//
            // White                                                          //
            //----------------------------------------------------------------//

            posY += _lineInc;

            PCLXLWriter.addAttrUbyte (ref bufStd,
                               ref indStd,
                               PCLXLAttributes.eTag.GrayLevel,
                               255);

            PCLXLWriter.addOperator (ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.SetBrushSource);

            prnWriter.Write (bufStd, 0, indStd);
            indStd = 0;

            PCLXLWriter.text (prnWriter, false, false,
                       PCLXLWriter.advances_Courier, ptSize,
                       posX, posY,
                       sampleText);

            //----------------------------------------------------------------//

            PCLXLWriter.addAttrUint16(ref bufStd,
                                ref indStd,
                                PCLXLAttributes.eTag.PageCopies,
                                1);

            PCLXLWriter.addOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.eTag.EndPage);

            prnWriter.Write(bufStd, 0, indStd);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p a t t e r n D e f i n e D p i 3 0 0                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Define 300 dots-per-inch user-defined patterns.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void patternDefineDpi300 (BinaryWriter prnWriter,
                                                 Boolean      formAsMacro)
        {
            const UInt16 patWidth = 16;
            const UInt16 patHeight = 16;

            const UInt16 destWidth =
                (UInt16) ((patWidth * _unitsPerInch) / 300);
            const UInt16 destHeight =
                (UInt16) ((patHeight * _unitsPerInch) / 300);

            Byte [] pattern_LightGrey = 
                               { 0x00, 0x00, 0x60, 0x60,
                                 0x60, 0x60, 0x00, 0x00,
                                 0x00, 0x00, 0x06, 0x06,
                                 0x06, 0x06, 0x00, 0x00,
                                 0x00, 0x00, 0x60, 0x60,
                                 0x60, 0x60, 0x00, 0x00,
                                 0x00, 0x00, 0x06, 0x06,
                                 0x06, 0x06, 0x00, 0x00 }; 
            
            Byte [] pattern_DarkGrey =
                               { 0xC1, 0xC1, 0xEB, 0xEB,
                                 0xC1, 0xC1, 0x88, 0x88,
                                 0x1C, 0x1C, 0xBE, 0xBE,
                                 0x1C, 0x1C, 0x88, 0x88,
                                 0xC1, 0xC1, 0xEB, 0xEB,
                                 0xC1, 0xC1, 0x88, 0x88,
                                 0x1C, 0x1C, 0xBE, 0xBE,
                                 0x1C, 0x1C, 0x88, 0x88 };

            PCLXLWriter.patternDefine (prnWriter,
                                       formAsMacro,
                                       _patternId_LightGrey,
                                       patWidth,
                                       patHeight,
                                       destWidth,
                                       destHeight,
                                       PCLXLAttrEnums.eVal.eIndexedPixel,
                                       PCLXLAttrEnums.eVal.e1Bit,
                                       PCLXLAttrEnums.eVal.ePagePattern,
                                       PCLXLAttrEnums.eVal.eNoCompression,
                                       pattern_LightGrey);

            PCLXLWriter.patternDefine (prnWriter,
                                       formAsMacro,
                                       _patternId_DarkGrey,
                                       patWidth,
                                       patHeight,
                                       destWidth,
                                       destHeight,
                                       PCLXLAttrEnums.eVal.eIndexedPixel,
                                       PCLXLAttrEnums.eVal.e1Bit,
                                       PCLXLAttrEnums.eVal.ePagePattern,
                                       PCLXLAttrEnums.eVal.eNoCompression,
                                       pattern_DarkGrey);
        }
    }
}
