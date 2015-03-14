using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColoEngine.Model;
using ColoEngine.Model.DirectionControl;
using ColoEngine.Model.BorderControl;
using ColoEngine.Model.LocationStatemachine;
using ColoEngine.Model.LookAheadPath;
using ColoEngine.Model.Init;
using System.Collections.Generic;

namespace ColoBoxTests
{
    [TestClass]
    public class DirectionTests : TestBase
    {
        #region PositiveAngleL2R
        [TestMethod]
        public void Y_TouchesBorderL2R_in_Limits_Positive()
        {
            //arrange
            DirectionL2R direction = new DirectionL2R();
            double maxY = -400.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderPositive));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderPositive), b);

            //assert
            Assert.AreEqual(result, maxY);
        }
        [TestMethod]
        public void Y_TouchesBorderL2R_above_Limits_Positive()
        {
            //arrange
            DirectionL2R direction = new DirectionL2R();
            double maxY = -380.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderPositive));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderPositive), b);

            //assert
            Assert.AreEqual(result, -400.92);
        }
        [TestMethod]
        public void Y_TouchesBorderL2R_below_Limits_Positive()
        {
            //arrange
            DirectionL2R direction = new DirectionL2R();
            double maxY = -520.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderPositive));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderPositive), b);

            //assert
            Assert.AreEqual(result, maxY);
        }
        #endregion PositiveAngleL2R

        #region negativeAngleL2R
        [TestMethod]
        public void Y_TouchesBorderL2R_in_Limits_Negative()
        {
            //arrange
            DirectionL2R direction = new DirectionL2R();
            double maxY = -400.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderNegative));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderNegative), b);

            //assert
            Assert.AreEqual(result, -420.92);
        }
        [TestMethod]
        public void Y_TouchesBorderL2R_above_Limits_Negative()
        {
            //arrange
            DirectionL2R direction = new DirectionL2R();
            
            double maxY = -380.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderNegative));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderNegative), b);

            //assert
            Assert.AreEqual(result, -400.92);
        }
        [TestMethod]
        public void Y_TouchesBorderL2R_below_Limits_Negative()
        {
            //arrange
            DirectionL2R direction = new DirectionL2R();
            double maxY = -520.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderNegative));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderNegative), b);

            //assert
            Assert.AreEqual(result, maxY);
        }
        #endregion negativeAngleL2R

        #region PositiveAngleR2L
        [TestMethod]
        public void Y_TouchesBorderR2L_in_Limits_Positive()
        {
            //arrange
            DirectionR2L direction = new DirectionR2L();
            double maxY = -400.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderPositive));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderPositive), b);

            //assert
            Assert.AreEqual(result, -420.92);
        }
        [TestMethod]
        public void Y_TouchesBorderR2L_above_Limits_Positive()
        {
            //arrange
            DirectionR2L direction = new DirectionR2L();
            double maxY = -380.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderPositive));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderPositive), b);

            //assert
            Assert.AreEqual(result, -400.92);
        }
        [TestMethod]
        public void Y_TouchesBorderR2L_below_Limits_Positive()
        {
            //arrange
            DirectionR2L direction = new DirectionR2L();
            double maxY = -520.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderPositive));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderPositive), b);

            //assert
            Assert.AreEqual(result, maxY);
        }
        #endregion PositiveAngleR2L

        #region negativeAngleR2L
        [TestMethod]
        public void Y_TouchesBorderR2L_in_Limits_Negative()
        {
            //arrange
            DirectionR2L direction = new DirectionR2L();
            double maxY = -400.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderNegative));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderNegative), b);

            //assert
            Assert.AreEqual(result, maxY);
        }
        [TestMethod]
        public void Y_TouchesBorderR2L_above_Limits_Negative()
        {
            //arrange
            DirectionR2L direction = new DirectionR2L();
            double maxY = -380.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderNegative));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderNegative), b);

            //assert
            Assert.AreEqual(result, -400.92);
        }
        [TestMethod]
        public void Y_TouchesBorderR2L_below_Limits_Negative()
        {
            //arrange
            DirectionR2L direction = new DirectionR2L();
            double maxY = -520.92;
            ColoBox currentBoxReference = getBox(maxY);
            TiltBorder b = getBorder(direction, typeof(FirstBorderNegative));

            //act
            double result = direction.GetYPointTouchesBorder(currentBoxReference, typeof(FirstBorderNegative), b);

            //assert
            Assert.AreEqual(result, maxY);
        }
        #endregion negativeAngleR2L


        #region GetFirstBorderCoordinate
        [TestMethod]
        public void GetNextLocation_Positive_L2R()
        {
            //arrange
            InitParams InitParam = new InitParams { ColdAisleWidth = 504 };
            PathSection pathSection = getPathSection(true);
            State_RowStart rowStart = new State_RowStart(pathSection.machine);
            pathSection.machine.InitParam = InitParam;
            pathSection.machine.RowNumber = 1;
            pathSection.machine.CurrentBoxReference = getBox(-200);

            //act
            Point p = rowStart.GetNextLocation();


            Assert.AreEqual(Math.Round(p.X, 2), 308.24);
            Assert.AreEqual(Math.Round(p.Y, 2), -392);

        }
        [TestMethod]
        public void GetNextLocation_Positive_R2L()
        {
            //arrange
            InitParams InitParam = new InitParams { ColdAisleWidth = 504 };
            PathSection pathSection = getPathSection(false);
            State_RowStart rowStart = new State_RowStart(pathSection.machine);
            pathSection.machine.InitParam = InitParam;
            pathSection.machine.RowNumber = 1;
            pathSection.machine.CurrentBoxReference = getBox(-200);

            //act
            Point p = rowStart.GetNextLocation();


            Assert.AreEqual(Math.Round(p.X, 2), 677.8);
            Assert.AreEqual(Math.Round(p.Y, 2), -392);
        }
        [TestMethod]
        public void GetFirstBorderCoordinatExtremeL2R()
        {
            //arrange
            PathSection pathSectionL2R = getPathSection(true);            
            ColoBox box = getBox(-9);
            box.MoveOriginTo(new Point { X = 153, Y = -28 });
            pathSectionL2R.machine.CurrentBoxReference = box;            

            //act
            double resultL2R = pathSectionL2R.machine.direction.GetFirstBorderCoordinate();            

            //assert
            Assert.AreEqual(resultL2R, double.MaxValue);            

        }
        [TestMethod]
        public void GetFirstBorderCoordinatExtremeR2L()
        {
            //arrange            
            PathSection pathSectionR2L = getPathSection(false);
            ColoBox box = getBox(-9);
            box.MoveOriginTo(new Point { X = 153, Y = -28 });            
            pathSectionR2L.machine.CurrentBoxReference = box;

            //act            
            double resultR2L = pathSectionR2L.machine.direction.GetFirstBorderCoordinate();

            //assert            
            Assert.AreEqual(resultR2L, double.MinValue);

        }
        [TestMethod]
        public void IsBoxOutside_Inside_CrazyColoL2R()
        {
            //arrange
            ColoBox box = getBox(-375);
            box.MoveOriginTo(new Point { X = 655, Y = -375 });
            PathSection pathSectionL2R = getPathSection(true);            
            pathSectionL2R.SetRelativeLocation(box);            

            //act
            bool isBoxL2ROutside = pathSectionL2R.machine.direction.IsBoxOutside(box, pathSectionL2R);            

            //assert
            Assert.AreEqual(isBoxL2ROutside, false);            
        }
        [TestMethod]
        public void IsBoxOutside_Inside_CrazyColoR2L()
        {
            //arrange
            ColoBox box = getBox(-375);
            box.MoveOriginTo(new Point { X = 655, Y = -375 });            
            PathSection pathSectionR2L = getPathSection(false);            
            pathSectionR2L.SetRelativeLocation(box);

            //act            
            bool isBoxR2LOutside = pathSectionR2L.machine.direction.IsBoxOutside(box, pathSectionR2L);

            //assert
            Assert.AreEqual(isBoxR2LOutside, false);
        }

        [TestMethod]
        public void IsBoxOutside_Outside_CrazyColoL2R()
        {
            //arrange
            ColoBox box = getBox(-375);
            box.MoveOriginTo(new Point { X = 692, Y = -395 });
            PathSection pathSectionL2R = getPathSection(true);            
            pathSectionL2R.SetRelativeLocation(box);            

            //act
            bool isBoxL2ROutside = pathSectionL2R.machine.direction.IsBoxOutside(box, pathSectionL2R);            

            //assert
            Assert.AreEqual(isBoxL2ROutside, true);            
        }
        [TestMethod]
        public void IsBoxOutside_Outside_CrazyColoR2L()
        {
            //arrange
            ColoBox box = getBox(-375);
            box.MoveOriginTo(new Point { X = 692, Y = -395 });            
            PathSection pathSectionR2L = getPathSection(false);            
            pathSectionR2L.SetRelativeLocation(box);

            //act            
            bool isBoxR2LOutside = pathSectionR2L.machine.direction.IsBoxOutside(box, pathSectionR2L);

            //assert            
            Assert.AreEqual(isBoxR2LOutside, true);
        }
        [TestMethod]
        public void _2_IsBoxOutside_inside_TouchL2R()
        {
            ColoBox box = getBox(-375);
            box.MoveOriginTo(new Point { X = 410, Y = 335 });
            PathSection pathSectionL2R = getPathSection(true, true);            
            pathSectionL2R.machine.CurrentBoxReference = box;
            pathSectionL2R.SetRelativeLocation(box);
            
            
            //act
            bool isBoxL2ROutside = pathSectionL2R.machine.direction.IsBoxOutside(box, pathSectionL2R);            

            //assert
            Assert.AreEqual(isBoxL2ROutside, false);
            
        }

        [TestMethod]
        public void IsBoxOutside_inside_TouchR2L()
        {
            ColoBox box = getBox(-375);
            box.MoveOriginTo(new Point { X = 410, Y = 335 });            
            PathSection pathSectionR2L = getPathSection(false, true);           
            pathSectionR2L.SetRelativeLocation(box);

            //act            
            bool isBoxR2LOutside = pathSectionR2L.machine.direction.IsBoxOutside(box, pathSectionR2L);

            //assert            
            Assert.AreEqual(isBoxR2LOutside, false);
        }
        //[TestMethod]
        public void GetNextLocation_Vertical_L2R()
        {
            throw new NotImplementedException();
        }        
        //[TestMethod]
        public void GetNextLocation_Negative_R2L()
        {
            throw new NotImplementedException();
        }
        //[TestMethod]
        public void GetNextLocation_Vertical_R2L()
        {
            throw new NotImplementedException();
        }


        #endregion 

        #region sortSectionOrder
        [TestMethod]
        public void SortSectionOrderR2L()
        {
            //arrange            
            List<Border> lst = new List<Border>();
            lst.Add(new Border { Coordinate = 639.3339 });
            lst.Add(new Border { Coordinate = 193.9599 });
            lst.Add(new Border { Coordinate = 638.5776 });
            DirectionR2L direction = new DirectionR2L();            
            
            //act
            List<Border> result = direction.SortLookaheadPath(lst);

            //assert
            Assert.AreEqual(result[0].Coordinate, 639.3339);
            Assert.AreEqual(result[1].Coordinate, 638.5776);
            Assert.AreEqual(result[2].Coordinate, 193.9599);
        }
        [TestMethod]
        public void SortSectionOrderL2R()
        {
            //arrange            
            List<Border> lst = new List<Border>();
            lst.Add(new Border { Coordinate = 639.3339 });
            lst.Add(new Border { Coordinate = 193.9599 });
            lst.Add(new Border { Coordinate = 638.5776 });
            DirectionL2R direction = new DirectionL2R();

            //act
            List<Border> result = direction.SortLookaheadPath(lst);

            //assert
            Assert.AreEqual(result[0].Coordinate, 193.9599);
            Assert.AreEqual(result[1].Coordinate, 638.5776);
            Assert.AreEqual(result[2].Coordinate, 639.3339);
            
            
        }
        #endregion sortSectionOrder
    }

    
}
