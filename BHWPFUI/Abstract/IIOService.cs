using BHWPF.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHWPF.UI.Abstract
{
    public interface IIOService
    {
        string OpenFileDialog();

        List<List<Coordinates>> OpenFile(string path,
            int LateralDimensionX, int LateralDimensionY,
            double GridCellSizeX, double GridCellSizeY,
            decimal TopHorizonDistanceMeter,
            decimal FluidContactDistanceMeter);

        double Calculate(int VolumeType, List<Coordinates> topHorizon, List<Coordinates> baseHorizon, int LateralDimensionX, int LateralDimensionY, double GridCellSizeY);
    }
}
