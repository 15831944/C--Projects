using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoEngine.Model;
using ColoEngine.Model.DirectionControl;
using ColoEngine.Model.BorderControl;
using ColoEngine.Model.LocationStatemachine;
using ColoEngine.Model.LookAheadPath;
using ColoEngine.Model.Init;

namespace ColoBoxTests
{
    public class TestBase
    {
        #region protected
        protected ColoBox getBox(double maxY,bool isTough = false)
        {
            int height = 20;
            if (isTough)
                height = 15;

            ColoBox box = new ColoBox();
            box.AddPoint(826.18, maxY - height);
            box.AddPoint(826.18, maxY);
            box.AddPoint(845.38, maxY - height);
            box.AddPoint(845.38, maxY);
            return box;
        }

        protected TiltBorder getBorder(DirectionBase direction, Type type)
        {
            ColoBox box = new ColoBox("", 1);
            Point p1 = new Point();
            Point p2 = new Point();
            if (type == typeof(FirstBorderPositive))
            {
                p1.X = 160.21;
                p2.X = 310;
            }
            else
            {
                p1.X = 310;
                p2.X = 160.21;
            }

            p1.Y = -530.45;
            p2.Y = -390;
            RawBorder rb = new RawBorder(p1, p2);

            TiltBorder tb = new TiltBorder(rb, box);
            direction.LocationMachine = new Machine(new InitParams { Tolerance = 0.5 }, null);
            return tb;
        }

        protected BorderVerifier getBorderVerifier(DirectionBase direction, Machine machine)
        {
            ColoBox boundingBox = new ColoBox("", 1);
            boundingBox.AddPoint(160.38, -530);
            boundingBox.AddPoint(310.38, -390);
            boundingBox.AddPoint(166.15, -272);
            boundingBox.AddPoint(256.54, -128);
            boundingBox.AddPoint(291.15, -128);
            boundingBox.AddPoint(312.31, -101);
            boundingBox.AddPoint(341.15, -101);
            boundingBox.AddPoint(362.31, -128);
            boundingBox.AddPoint(387.31, -128);
            boundingBox.AddPoint(402.69, -99);
            boundingBox.AddPoint(472.42, -99);
            boundingBox.AddPoint(492.5, -130);
            boundingBox.AddPoint(788, -130);
            boundingBox.AddPoint(788, -379);
            boundingBox.AddPoint(697, -379);
            boundingBox.AddPoint(697, -420);
            boundingBox.AddPoint(639, -420);
            boundingBox.AddPoint(639, -499);
            boundingBox.AddPoint(831.54, -692);
            boundingBox.AddPoint(716.15, -849);
            boundingBox.AddPoint(553, -849);
            boundingBox.AddPoint(494.25, -896);
            boundingBox.AddPoint(441.58, -896);
            boundingBox.AddPoint(386.33, -849);
            boundingBox.AddPoint(273.85, -849);
            boundingBox.AddPoint(160.38, -530);
            List<ColoBox> staticBoxes = new List<ColoBox>();
            staticBoxes.Add(boundingBox);
            machine.boundingBox = boundingBox;
            BorderVerifier verifier = new BorderVerifier(staticBoxes, machine);

            return verifier;
        }

        protected BorderVerifier getToughBorderVerifier(DirectionBase direction, Machine machine)
        {
            ColoBox boundingBox = new ColoBox("", 1);
            boundingBox.AddPoint(531.67,415);
            boundingBox.AddPoint(531.67,474.61);
            boundingBox.AddPoint(507.86,497.33);
            boundingBox.AddPoint(465.8,497.33);
            boundingBox.AddPoint(463.07,494.76);
            boundingBox.AddPoint(459.83,491.71);
            boundingBox.AddPoint(456,488.1);
            boundingBox.AddPoint(456,473.67);
            boundingBox.AddPoint(473.67,473.67);
            boundingBox.AddPoint(473.67,445.67);
            boundingBox.AddPoint(456,445.67);
            boundingBox.AddPoint(456,415);
            boundingBox.AddPoint(410,415);
            boundingBox.AddPoint(410,335);
            boundingBox.AddPoint(483,335);
            boundingBox.AddPoint(483,352.67);
            boundingBox.AddPoint(532,352.67);
            boundingBox.AddPoint(532,335);
            boundingBox.AddPoint(595.33,335);
            boundingBox.AddPoint(595.33,415);
            boundingBox.AddPoint(531.67,415);
            List<ColoBox> staticBoxes = new List<ColoBox>();
            staticBoxes.Add(boundingBox);
            machine.boundingBox = boundingBox;
            BorderVerifier verifier = new BorderVerifier(staticBoxes, machine);

            return verifier;
        }

        protected PathSection getPathSection(bool IsL2R,bool isTough = false)
        {
            InitParams init = new InitParams();
            ColoBox box = new ColoBox("",1);
            Machine machine = new Machine(init, box);
            PathSection pathSection = new PathSection(machine);
            DirectionBase direction = null;
            if (IsL2R)
                direction = new DirectionL2R(machine);
            else
                direction = new DirectionR2L(machine);

            LayoutManager layoutManager = new LayoutManager(init);
            layoutManager.LocationMachine = machine;
            machine.direction = direction;
            machine.direction.LayoutManager = layoutManager;
            if(isTough)
                machine.BorderVerifier = getToughBorderVerifier(direction, machine);
            else
                machine.BorderVerifier = getBorderVerifier(direction, machine);
            
            return pathSection;
        }
        #endregion
    }
}
