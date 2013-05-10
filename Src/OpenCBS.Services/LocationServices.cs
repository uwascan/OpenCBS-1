﻿// LICENSE PLACEHOLDER

using System;
using System.Collections.Generic;
using OpenCBS.CoreDomain;
using OpenCBS.ExceptionsHandler;
using OpenCBS.Manager;


namespace OpenCBS.Services
{
    public class LocationServices : MarshalByRefObject
    {
        private readonly LocationsManager _locationsManager;

        public LocationServices(User pUser)
        {
            _locationsManager = new LocationsManager(pUser);
        }

        public LocationServices(LocationsManager pLocationsManager)
        {
            _locationsManager = pLocationsManager;
        }

        public List<Province> GetProvinces()
        {
            return _locationsManager.GetProvinces();
        }

        public List<District> GetDistricts()
        {
            return _locationsManager.GetDistricts();
        }

        public List<City> GetCities()
        {
            return _locationsManager.GetCities();
        }

        public int AddProvince(string name)
        {
            CheckLocation(name);
            return _locationsManager.AddProvince(name);
        }

        private static void CheckLocation(string name)
        {
            if (name == null || name.Trim(' ') == string.Empty)
                throw new OpenCbsEmptyLocationException();
        }

        public bool UpdateProvince(Province province)
        {
            CheckLocation(province.Name);
            return _locationsManager.UpdateProvince(province);
        }

        public bool DeleteProvince(Province pProvince)
        {
            bool deleteProvincePossible = true;
            List<District> districts = new List<District>();
            districts = _locationsManager.GetDistricts();
            for (int i = 0; i < districts.Count; i++)
            {
                if (districts[i].Province.Id != pProvince.Id)
                    continue;
                deleteProvincePossible = false;
                break;
            }
            if (deleteProvincePossible)
                _locationsManager.DeleteProvinceById(pProvince.Id);
            return deleteProvincePossible;
        }

        public int AddDistrict(string name, int pProvinceId)
        {
            CheckLocation(name);
            return _locationsManager.AddDistrict(name, pProvinceId);
        }

        public bool UpdateDistrict(District district)
        {
            CheckLocation(district.Name);
            return _locationsManager.UpdateDistrict(district);
        }

        public bool DeleteDistrict(int districtId)
        {
            bool deleteDistrictPossible = true;
            List<City> cities = new List<City>();
            cities = _locationsManager.GetCities();
            for (int i = 0; i < cities.Count; i++)
            {
                if (cities[i].DistrictId != districtId)
                    continue;
                deleteDistrictPossible = false;
                break;
            }
            CheckDistrictToDelete(districtId);
            if (deleteDistrictPossible)
                _locationsManager.DeleteDistrictById(districtId);
            return deleteDistrictPossible;
        }

        private void CheckDistrictToDelete(int districtId)
        {
            if (_locationsManager.IsDistrictUsed(districtId))
                throw new OpenCbsDistrictUsedException();
        }

        public int AddCity(City city)
        {
            CheckLocation(city.Name);
            return _locationsManager.AddCity(city);
        }

        public void DeleteCity(int cityId)
        {
            CheckCityToDelete(cityId);
            _locationsManager.DeleteCityById(cityId);
        }

        private void CheckCityToDelete(int cityId)
        {
            if (_locationsManager.IsCityUsed(cityId))
                throw new OpenCbsCityUsedException();
        }

        public bool UpdateCity(City city)
        {
            CheckLocation(city.Name);
            return _locationsManager.UpdateCity(city);
        }

        public List<Province> FindAllProvinces()
        {
            return _locationsManager.SelectAllProvinces();
        }

        public List<City> FindCityByDistrictId(int districtId)
        {
            return _locationsManager.SelectCityByDistrictId(districtId);
        }

        public List<District> FindDistrict(Province province)
        {
            return province.Id == 0 ? _locationsManager.GetDistricts() : _locationsManager.SelectDistrictsByProvinceId(province.Id);
        }

        public Province FindProvinceByName(string name)
        {
            return _locationsManager.SelectProvinceByName(name);
        }

        public District FindDistirctByName(string name)
        {
            return _locationsManager.SelectDistrictByName(name);
        }

        public District FindDistrictByCityName(string name)
        {
            return _locationsManager.SelectDistrictByCityName(name);
        }
    }
}
