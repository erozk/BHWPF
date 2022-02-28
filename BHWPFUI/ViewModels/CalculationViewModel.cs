using AutoMapper;
using BHWPF.Data.Repository;
using BHWPF.UI.Abstract;
using BHWPF.UI.Base;
using BHWPF.UI.Helpers;
using BHWPF.UI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace BHWPF.UI.ViewModels
{
    public class CalculationViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private IMapper _imapper;
        private ICalculationRepository _ical;
        private IIOService _ioService;

        public ICommand OnCalculationCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand OnClearFileCommand { get; set; }

        public CalculationViewModel(IMapper imapper, ICalculationRepository ical, IIOService ioService, ILogger logger)
        {
            _imapper = imapper;
            _ical = ical;
            _ioService = ioService;
            _logger = logger;

            LoadCalculationTypes();

            OnCalculationCommand = new RelayCommand(Calculate);
            OpenFileCommand = new RelayCommand(OpenFile);
            OnClearFileCommand = new RelayCommand(ClearFile);
        }


        public void Calculate()
        {
            try
            {
                double volume = _ioService.Calculate(int.Parse(SelectedCalculation), DepthPoints[0], DepthPoints[1], Convert.ToInt32(LateralDimensionX), Convert.ToInt32(LateralDimensionY), Convert.ToDouble(GridCellSizeY));
                VolumeResult = volume.ToString();
                _logger.LogInformation("Calculation Successfull");
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Calculate Error:{0}", ex.Message));
            }  
        }

        public void OpenFile()
        {
            SelectedPath = _ioService.OpenFileDialog();
            if (SelectedPath == null)
            {
                SelectedPath = string.Empty;
                SelectedFile = "No file selected";
                IsClearFileButtonVisible = false;
            }
            else
            {
                try
                {
                    DepthPoints = _ioService.OpenFile(SelectedPath, Convert.ToInt32(LateralDimensionX),
                            Convert.ToInt32(LateralDimensionY), Convert.ToDouble(GridCellSizeX),
                            Convert.ToDouble(GridCellSizeY),
                            Convert.ToDecimal(TopHorizonDistanceMeter),
                            Convert.ToDecimal(FluidContactDistanceMeter));

                    SelectedFile = Path.GetFileName(SelectedPath);

                    IsClearFileButtonVisible = true;

                    //DepthPoints needs to have base and top horizon coordinates in 2 List. If Lateral Dimension X and Y doesnt fit to file data, button not enable

                    if (DepthPoints.Count == 0)
                    {
                        _logger.LogError("File data is not in proper format");
                        VolumeResult = "Check Data Format!";
                    }
                    else
                    { 
                        _logger.LogInformation("Coordinates of Top and Base Horizons created");
                        VolumeResult = string.Empty;
                    }

                }
                catch (Exception ex)
                {
                    SelectedFile = "No file selected";
                    _logger.LogError(string.Format("Open File Error:{0}", ex.Message));
                }
            }
        }


        public void ClearFile()
        {
            SelectedPath = string.Empty;
            SelectedFile = "No file selected";
            IsClearFileButtonVisible = false;
            VolumeResult = string.Empty;
        }

        // X,Y,Z coordinates of base and top horizon layers till FluidContactDistanceMeter

        private List<List<Coordinates>> depthPoints; 

        public List<List<Coordinates>> DepthPoints
        {
            get { return depthPoints; }
            set { depthPoints = value; }
        }



        public void LoadCalculationTypes()
        {
            List<CalculationTypesModel> types = _imapper.Map<List<CalculationTypesModel>>(_ical.GetCalculationTypes());
            CalculationTypes = new BindingList<CalculationTypesModel>(types);
        }


        private BindingList<CalculationTypesModel> _calculationTypes;

        public BindingList<CalculationTypesModel> CalculationTypes
        {
            get { return _calculationTypes; }
            set
            {
                _calculationTypes = value;
                CallChangedEvent(nameof(CalculationTypes));
            }
        }

        private string _selectedCalculation = "1";

        public string SelectedCalculation
        {
            get { return _selectedCalculation; }
            set {
                _selectedCalculation = value;
                CallChangedEvent(nameof(SelectedCalculation));
            }
        }

        private string _lateralDimensionX = "16";

        public string LateralDimensionX
        {
            get { return _lateralDimensionX; }
            set
            {
                _lateralDimensionX = value;
                CallChangedEvent(nameof(LateralDimensionX));
                CallChangedEvent(nameof(IsCalculationEnabled));
            }
        }

        private string _lateralDimensionY = "26";

        public string LateralDimensionY
        {
            get { return _lateralDimensionY; }
            set
            {
                _lateralDimensionY = value;
                CallChangedEvent(nameof(LateralDimensionY));
                CallChangedEvent(nameof(IsCalculationEnabled));
            }
        }


        private string _gridCellSizeX = "200";

        public string GridCellSizeX
        {
            get { return _gridCellSizeX; }
            set
            {
                _gridCellSizeX = value;
                CallChangedEvent(nameof(GridCellSizeX));
                CallChangedEvent(nameof(IsCalculationEnabled));
            }
        }

        private string _gridCellSizeY = "200";

        public string GridCellSizeY
        {
            get { return _gridCellSizeY; }
            set
            {
                _gridCellSizeY = value;
                CallChangedEvent(nameof(GridCellSizeY));
                CallChangedEvent(nameof(IsCalculationEnabled));
            }
        }

        private string _topHorizonDistanceMeter = "100";

        public string TopHorizonDistanceMeter
        {
            get { return _topHorizonDistanceMeter; }
            set
            {
                _topHorizonDistanceMeter = value;
                CallChangedEvent(nameof(TopHorizonDistanceMeter));
                CallChangedEvent(nameof(IsCalculationEnabled));
            }
        }

        private string _fluidContactDistanceMeter = "3000";

        public string FluidContactDistanceMeter
        {
            get { return _fluidContactDistanceMeter; }
            set
            {
                _fluidContactDistanceMeter = value;
                CallChangedEvent(nameof(FluidContactDistanceMeter));
                CallChangedEvent(nameof(IsCalculationEnabled));
            }
        }

        private string _volumeResult;

        public string VolumeResult
        {
            get { return _volumeResult; }
            set
            {
                _volumeResult = value;
                CallChangedEvent(nameof(VolumeResult));
            }
        }

        private string _selectedPath;

        public string SelectedPath
        {
            get { return _selectedPath; }
            set { _selectedPath = value; CallChangedEvent(nameof(SelectedPath)); }
        }

        private string _selectedFile = "No file selected";

        public string SelectedFile
        {
            get { return _selectedFile; }
            set { _selectedFile = value; CallChangedEvent(nameof(SelectedFile)); CallChangedEvent(nameof(IsCalculationEnabled)); CallChangedEvent(nameof(IsClearFileButtonVisible)); }
        }

        private bool _isCalculationEnabled = false;

        public bool IsCalculationEnabled
        {
            get
            {
                if (SelectedFile=="No file selected" || DepthPoints.Count==0 || string.IsNullOrEmpty(LateralDimensionX) || string.IsNullOrEmpty(LateralDimensionY) || string.IsNullOrEmpty(GridCellSizeX) || string.IsNullOrEmpty(GridCellSizeY) || string.IsNullOrEmpty(SelectedCalculation) || string.IsNullOrEmpty(FluidContactDistanceMeter) || string.IsNullOrEmpty(TopHorizonDistanceMeter))
                {
                    _isCalculationEnabled = false;
                }
                else
                {
                    _isCalculationEnabled = true;
                }
                return _isCalculationEnabled;
            }
            set { _isCalculationEnabled = value; CallChangedEvent(nameof(IsCalculationEnabled)); }
        }

        private bool _isClearFileButtonVisible = false;

        public bool IsClearFileButtonVisible
        {
            get { return _isClearFileButtonVisible; }
            set { _isClearFileButtonVisible  = value; CallChangedEvent(nameof(IsClearFileButtonVisible)); }
        }


    }
}
