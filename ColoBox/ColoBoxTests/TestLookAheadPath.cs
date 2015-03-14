using ColoEngine.Model;
using ColoEngine.Model.BorderControl;
using ColoEngine.Model.DirectionControl;
using ColoEngine.Model.LocationStatemachine;
using ColoEngine.Model.LookAheadPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoBoxTests
{
    [TestClass]
    public class TestLookAheadPath : TestBase
    {
        [TestMethod]
        public void LookAhead_Top2SectionsPositiveNegative_NegativePositive()
        {
            //arrange
            PathSection pathSection = getPathSection(true);            
            double maxY = -102.92;
            ColoBox currentBoxReference = getBox(maxY);
            
            //act
            pathSection.SetRelativeLocation(currentBoxReference);

            //assert
            Assert.AreEqual(pathSection.UpperPathSections.Count, 3);
            Assert.AreEqual(pathSection.LowerPathSections.Count, 3);
            //upper
            Assert.AreEqual(Math.Round(pathSection.UpperPathSections[0].X, 2), 310.81);
            Assert.AreEqual(Math.Round(pathSection.UpperPathSections[0].Y, 2), 342.65);
            Assert.AreEqual(pathSection.UpperPathSections[0].RelativeLocation, RelativeLocation.Inside);

            Assert.AreEqual(Math.Round(pathSection.UpperPathSections[1].X, 2), 342.65);
            Assert.AreEqual(Math.Round(pathSection.UpperPathSections[1].Y, 2), 400.61);
            Assert.AreEqual(pathSection.UpperPathSections[1].RelativeLocation, RelativeLocation.Outside);

            Assert.AreEqual(Math.Round(pathSection.UpperPathSections[2].X, 2), 400.61);
            Assert.AreEqual(Math.Round(pathSection.UpperPathSections[2].Y, 2), 474.96);
            Assert.AreEqual(pathSection.UpperPathSections[2].RelativeLocation, RelativeLocation.Inside);

            //lower
            Assert.AreEqual(Math.Round(pathSection.LowerPathSections[0].X, 2), 295.13);
            Assert.AreEqual(Math.Round(pathSection.LowerPathSections[0].Y, 2), 358.33);
            Assert.AreEqual(pathSection.LowerPathSections[0].RelativeLocation, RelativeLocation.Inside);

            Assert.AreEqual(Math.Round(pathSection.LowerPathSections[1].X, 2), 358.33);
            Assert.AreEqual(Math.Round(pathSection.LowerPathSections[1].Y, 2), 390);
            Assert.AreEqual(pathSection.LowerPathSections[1].RelativeLocation, RelativeLocation.Outside);

            Assert.AreEqual(Math.Round(pathSection.LowerPathSections[2].X, 2), 390);
            Assert.AreEqual(Math.Round(pathSection.LowerPathSections[2].Y, 2), 487.91);
            Assert.AreEqual(pathSection.LowerPathSections[2].RelativeLocation, RelativeLocation.Inside);
        }        

        [TestMethod]
        public void LookAhead_2VerticalsAndNegative()
        {
            //arrange
            PathSection pathSection = getPathSection(true);
            double maxY = -381.92;
            ColoBox currentBoxReference = getBox(maxY);

            //act
            pathSection.SetRelativeLocation(currentBoxReference);

            //assert
            Assert.AreEqual(pathSection.UpperPathSections.Count, 1);
            Assert.AreEqual(pathSection.LowerPathSections.Count, 1);
            //upper
            Assert.AreEqual(Math.Round(pathSection.UpperPathSections[0].X, 2), 300.5);
            Assert.AreEqual(Math.Round(pathSection.UpperPathSections[0].Y, 2), 697 );
            Assert.AreEqual(pathSection.UpperPathSections[0].RelativeLocation, RelativeLocation.Inside);

            //upper
            Assert.AreEqual(Math.Round(pathSection.LowerPathSections[0].X, 2),297.61 );
            Assert.AreEqual(Math.Round(pathSection.LowerPathSections[0].Y, 2),697 );
            Assert.AreEqual(pathSection.LowerPathSections[0].RelativeLocation, RelativeLocation.Inside);
        }

        [TestMethod]
        public void LookAhead_BeginingL2RMeet2AngledLines()
        {
            //arrange
            PathSection pathSection = getPathSection(true);
            double maxY = -517.92;//-529.92;
            ColoBox currentBoxReference = getBox(maxY);

            //act
            pathSection.SetRelativeLocation(currentBoxReference);

            Assert.AreEqual(pathSection.UpperPathSections.Count, 1);
            Assert.AreEqual(pathSection.LowerPathSections.Count, 1);
        }

        [TestMethod]
        public void UpdateMachineStateL2RPath1()
        {
            //arrange
            PathSection pathSection = getPathSection(true, true);
            double maxY = 335;
            ColoBox currentBoxReference = getBox(maxY, true);
            currentBoxReference.MoveOriginTo(new Point { X = 490, Y = maxY });
            pathSection.machine.CurrentBoxToPlace = currentBoxReference;

            //act
            Point? res = pathSection.UpdateMachineState(currentBoxReference);

            //assert
            Assert.AreEqual(res.Value.X, 532);
            Assert.AreEqual(res.Value.Y, 335);
        }

        [TestMethod]
        public void UpdateMachineStateL2RPath2()
        {
            //arrange
            PathSection pathSection = getPathSection(true, true);
            double maxY = 335;
            ColoBox currentBoxReference = getBox(maxY, true);
            currentBoxReference.MoveOriginTo(new Point { X = 482, Y = maxY });
            pathSection.machine.CurrentBoxToPlace = currentBoxReference;

            //act
            Point? res = pathSection.UpdateMachineState(currentBoxReference);

            //assert
            Assert.AreEqual(res,null);           
            Assert.AreEqual(pathSection.machine.IsInsideBoundingBox, false);
        }

        [TestMethod]
        public void UpdateMachineStateL2RPath3()
        {
            //arrange
            PathSection pathSection = getPathSection(true, true);
            double maxY = 563.92;//-529.92;
            ColoBox currentBoxReference = getBox(maxY);
            currentBoxReference.MoveOriginTo(new Point { X = 490, Y = 335 });
            pathSection.machine.CurrentBoxToPlace = currentBoxReference;

            //act
            Point? res = pathSection.UpdateMachineState(currentBoxReference);

            //assert
            Assert.AreEqual(res.Value.X, 532);
            Assert.AreEqual(res.Value.Y, 335);
        }

        [TestMethod]
        public void UpdateMachineStateL2RPath4()
        {
            //arrange
            PathSection pathSection = getPathSection(true, true);
            double maxY = 563.92;//-529.92;
            ColoBox currentBoxReference = getBox(maxY);
            currentBoxReference.MoveOriginTo(new Point { X = 464, Y = 335 });
            pathSection.machine.CurrentBoxToPlace = currentBoxReference;

            //act
            Point? res = pathSection.UpdateMachineState(currentBoxReference);

            //assert
            Assert.AreEqual(res, null);
            Assert.AreEqual(pathSection.machine.IsInsideBoundingBox, false);
        }

        [TestMethod]
        public void UpdateMachineStateR2LPath1()
        {
            //arrange
            PathSection pathSection = getPathSection(false, true);
            double maxY = 335;
            ColoBox currentBoxReference = getBox(maxY, true);
            currentBoxReference.MoveOriginTo(new Point { X = 490, Y = maxY });
            pathSection.machine.CurrentBoxToPlace = currentBoxReference;

            //act
            Point? res = pathSection.UpdateMachineState(currentBoxReference);

            //assert
            Assert.AreEqual(Math.Round(res.Value.X,2), 463.8);
            Assert.AreEqual(res.Value.Y, 335);
            Assert.AreEqual(pathSection.machine.IsInsideBoundingBox, true);
        }

        [TestMethod]
        public void UpdateMachineStateR2LPath2()
        {
            //arrange
            PathSection pathSection = getPathSection(false, true);
            double maxY = 335;
            ColoBox currentBoxReference = getBox(maxY, true);
            currentBoxReference.MoveOriginTo(new Point { X = 530, Y = maxY });
            pathSection.machine.CurrentBoxToPlace = currentBoxReference;

            //act
            Point? res = pathSection.UpdateMachineState(currentBoxReference);

            //assert
            Assert.AreEqual(res, null);
            Assert.AreEqual(pathSection.machine.IsInsideBoundingBox, false);
        }

        [TestMethod]
        public void UpdatemachineStateL2RBoxEndupOutside()
        {
            //arrange
            PathSection pathSection = getPathSection(true, true);
            double maxY = 415;
            ColoBox currentBoxReference = getBox(maxY, true);
            currentBoxReference.MoveOriginTo(new Point { X = 456, Y = maxY });
            pathSection.machine.CurrentBoxToPlace = currentBoxReference;

            //act
            Point? res = pathSection.UpdateMachineState(currentBoxReference);

            //assert
            Assert.AreEqual(pathSection.LowerPathSections[0].RelativeLocation.Value, RelativeLocation.Outside);
            Assert.AreEqual(pathSection.LowerPathSections[1].RelativeLocation.Value, RelativeLocation.Inside);
            Assert.AreEqual(pathSection.LowerPathSections[2].RelativeLocation.Value, RelativeLocation.Outside);
        }

        [TestMethod]
        public void UpdatemachineStateR2LBoxEndupOutside()
        {
            //arrange
            PathSection pathSection = getPathSection(false, true);
            double maxY = 415;
            ColoBox currentBoxReference = getBox(maxY, true);
            currentBoxReference.MoveOriginTo(new Point { X = 456, Y = maxY });
            pathSection.machine.CurrentBoxToPlace = currentBoxReference;

            //act
            Point? res = pathSection.UpdateMachineState(currentBoxReference);

            //assert
            Assert.AreEqual(pathSection.LowerPathSections[0].RelativeLocation.Value, RelativeLocation.Outside);
            Assert.AreEqual(pathSection.LowerPathSections[1].RelativeLocation.Value, RelativeLocation.Inside);
            Assert.AreEqual(pathSection.LowerPathSections[2].RelativeLocation.Value, RelativeLocation.Outside);
        }

        [TestMethod]
        public void _1_UpdatemachineStateL2RBoxEndupOutside2()
        {
            //arrange
            PathSection pathSection = getPathSection(true, true);
            ColoBox currentBoxReference = getBox(0, true);
            currentBoxReference.MoveOriginTo(new Point { X = 456, Y = 459 });
            pathSection.machine.CurrentBoxToPlace = currentBoxReference;

            //act
            Point? res = pathSection.UpdateMachineState(currentBoxReference);

            //assert            
            Assert.AreEqual(pathSection.UpperPathSections[0].RelativeLocation.Value, RelativeLocation.Outside);
            Assert.AreEqual(pathSection.UpperPathSections[1].RelativeLocation.Value, RelativeLocation.Inside);            
        }
    }
}
