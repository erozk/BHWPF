using BHWPF.UI.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BHWPF.UI.Abstract
{
    public class IOService : IIOService
    {

        public List<List<Coordinates>> OpenFile(string path,
            int LateralDimensionX, int LateralDimensionY,
            double GridCellSizeX, double GridCellSizeY,
            decimal TopHorizonDistanceMeter,
            decimal FluidContactDistanceMeter)
        {
            string[] depthValues;
            char[] separator = { ' ' };
            string str = "";

            double GridCellSizeXInMeter = GridCellSizeX / 3.281;
            double GridCellSizeYInMeter = GridCellSizeY / 3.281;

            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            List<Coordinates> coorListTillFluidContact = new List<Coordinates>(); // Coordinates of the layer which is between top horizon and fluid contact

            List<Coordinates> coorListTillBaseHorizon = new List<Coordinates>();  // Coordinates of the layer which is between base and fluid contact (base should be above fluid otherwise 0 point)


            int rowNr = 0;

            bool fileHasFault = false;

            while ((str = sr.ReadLine()) != null)
            {
                // top horizon depth values  (like a rectangle with 16 row and 26 column)

                // each readline will give us the rectangle's rows

                depthValues = str.Split(separator);

                fileHasFault = depthValues.Length != LateralDimensionX ? true : false;

                if (fileHasFault)
                {
                    break;
                }

                double distanceY = (rowNr + 1) == LateralDimensionY ? (LateralDimensionY * GridCellSizeYInMeter) - (GridCellSizeYInMeter / 2) : (((rowNr + 1) % LateralDimensionY) * GridCellSizeYInMeter) - (GridCellSizeYInMeter / 2);

                for (int i = 0; i < LateralDimensionX; i++)
                {
                    Coordinates coordinatesFromTopToFluidContact = new Coordinates();

                    Coordinates coordinatesOfBaseHorizonLayer = new Coordinates();

                    double distanceX = (i + 1) == LateralDimensionX ? (LateralDimensionX * GridCellSizeXInMeter) - (GridCellSizeXInMeter / 2) : (((i + 1) % LateralDimensionX) * GridCellSizeXInMeter) - (GridCellSizeXInMeter / 2);

                    double distanceZ = Convert.ToDouble(depthValues[i]) / 3.281;

                    coordinatesFromTopToFluidContact.X = distanceX;
                    coordinatesFromTopToFluidContact.Y = distanceY;
                    coordinatesFromTopToFluidContact.Z = distanceZ > Convert.ToDouble(FluidContactDistanceMeter) ? 0 : (Convert.ToDouble(FluidContactDistanceMeter) - distanceZ);
                    coorListTillFluidContact.Add(coordinatesFromTopToFluidContact);


                    coordinatesOfBaseHorizonLayer.X = distanceX;
                    coordinatesOfBaseHorizonLayer.Y = distanceY;
                    coordinatesOfBaseHorizonLayer.Z = distanceZ + Convert.ToDouble(TopHorizonDistanceMeter) > Convert.ToDouble(FluidContactDistanceMeter) ? 0 : (Convert.ToDouble(FluidContactDistanceMeter) - (distanceZ + Convert.ToDouble(TopHorizonDistanceMeter)));
                    coorListTillBaseHorizon.Add(coordinatesOfBaseHorizonLayer);
                }
                rowNr++;
            }

            // if fileHasFault not set due to row count, check for column count
            if (!fileHasFault)
            {
                fileHasFault = rowNr != LateralDimensionY ? true : false;
            }

            List<List<Coordinates>> bothLayerCoordinates = new List<List<Coordinates>>();


            // if hasfault true DepthValues will have no item to calculate. Use it for calculate button enable state check
            if (!fileHasFault)
            {
                bothLayerCoordinates.Add(coorListTillFluidContact);
                bothLayerCoordinates.Add(coorListTillBaseHorizon);
            }

            return bothLayerCoordinates;
        }

        public string OpenFileDialog()
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "CSV Files|*.csv";
            d.FilterIndex = 1;

            d.Multiselect = false;
            d.ShowDialog();

            return d.FileName;

        }

        public double Calculate(int VolumeType, List<Coordinates> topHorizon, List<Coordinates> baseHorizon, int LateralDimensionX, int LateralDimensionY, double GridCellSizeY)
        {
            // top horizon volume - base horizon volume calculation in cubic meter

            double topHorizonVolume = 0;

            double baseHorizonVolume = 0;

            double GridCellSizeYInMeter = GridCellSizeY / 3.281;

            for (int i = 0; i < LateralDimensionY - 1; i++)
            {
                double rowArea = GetAreaByCrossMethod(topHorizon.GetRange(i * LateralDimensionX, LateralDimensionX));
                double nextRowArea = GetAreaByCrossMethod(topHorizon.GetRange((i + 1) * LateralDimensionX, LateralDimensionX));

                double volumeBetweenRows = ((rowArea + nextRowArea) / 2) * GridCellSizeYInMeter;

                topHorizonVolume += volumeBetweenRows;

                double rowAreaBase = GetAreaByCrossMethod(baseHorizon.GetRange(i * LateralDimensionX, LateralDimensionX));
                double nextRowAreaBase = GetAreaByCrossMethod(baseHorizon.GetRange((i + 1) * LateralDimensionX, LateralDimensionX));

                double volumeBetweenRowsBase = ((rowAreaBase + nextRowAreaBase) / 2) * GridCellSizeYInMeter;

                baseHorizonVolume += volumeBetweenRowsBase;
            }

            if (VolumeType == 1) // cubicmeter
            {
                return topHorizonVolume - baseHorizonVolume;
            }
            else if (VolumeType == 2) // cubicfeet
            {
                return (topHorizonVolume - baseHorizonVolume) * 35.3146667;
            }
            else if (VolumeType == 3) // barrel
            {
                return (topHorizonVolume - baseHorizonVolume) * 8.38641436;
            }
            else
            {
                return -1;
            }

            
        }


        // calculate as square meter first

        public double GetAreaByCrossMethod(List<Coordinates> rowOfHorizon)
        {
            //rowOfHorizon.ForEach(item => Console.WriteLine(item.X + " " + item.Y + " " + item.Z));
            var area = Math.Abs(rowOfHorizon.Take(rowOfHorizon.Count - 1)
                .Select((p, i) => (rowOfHorizon[i + 1].X - p.X) * (rowOfHorizon[i + 1].Z + p.Z))
                .Sum() / 2);

            return area;
        }

    }
}
