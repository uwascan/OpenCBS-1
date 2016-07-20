﻿// Octopus MFS is an integrated suite for managing a Micro Finance Institution: 
// clients, contracts, accounting, reporting and risk
// Copyright © 2006,2007 OCTO Technology & OXUS Development Network
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//
// Website: http://www.opencbs.com
// Contact: contact@opencbs.com

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using OpenCBS.CoreDomain.Accounting;
using OpenCBS.CoreDomain.Contracts.Loans.Installments;
using OpenCBS.CoreDomain.Products;
using OpenCBS.Enums;
using OpenCBS.ExceptionsHandler;
using OpenCBS.GUI.UserControl;
using OpenCBS.MultiLanguageRessources;
using OpenCBS.Services;
using OpenCBS.Shared;
using OpenCBS.GUI.Tools;

namespace OpenCBS.GUI.Configuration
{
    public partial class FrmAddSavingBookProduct : SweetBaseForm
    {
        private SavingsBookProduct _savingsProduct;
        private int clientTypeCounter=0;
        private string _cash;
        private string _cheque;

        public FrmAddSavingBookProduct()
        {
            _savingsProduct = new SavingsBookProduct();
            InitializeComponent();
            InitializeComboBoxCurrencies();
            LoadInterestComboBox();
            LoadManagementFeeFreq();
            LoadAgioFeesFrequency();
            InitializeClientTypes();
            LoadPostingFrequency();
            InitializeTabPageTermDeposit();
            _savingsProduct.PackageMode = OPackageMode.Insert;

            rbFlatWithdrawFees_CheckedChanged(this, null);

            _personalAccountRadioButton.Checked = true;
            tabControlSaving.TabPages.Remove(tabPageTermDeposit);
        }

        public FrmAddSavingBookProduct(SavingsBookProduct product)
        {
            InitializeComponent();
            _savingsProduct = product;
            InitializeComboBoxCurrencies();
            LoadInterestComboBox();
            LoadManagementFeeFreq();
            LoadAgioFeesFrequency();
            InitializeClientTypes();
            InitializeControls(product);
            LoadPostingFrequency();
            InitializeTabPageTermDeposit();

            _savingsProduct.PackageMode = OPackageMode.Edit;

            _personalAccountRadioButton.Checked = true;
            tabControlSaving.TabPages.Remove(tabPageTermDeposit);
        }

        private void InitializeControls(SavingsBookProduct product)
        {
            cbCurrency.SelectedItem = _savingsProduct.Currency;
            tbName.Text = _savingsProduct.Name;
            tbCodeSavingProduct.Text = _savingsProduct.Code;
            tbBalanceMax.Text = _savingsProduct.BalanceMax.GetFormatedValue(_savingsProduct.UseCents);
            tbBalanceMin.Text = _savingsProduct.BalanceMin.GetFormatedValue(_savingsProduct.UseCents);
            tbDrawingMax.Text = _savingsProduct.WithdrawingMax.GetFormatedValue(_savingsProduct.UseCents);
            tbWithDrawingMin.Text = _savingsProduct.WithdrawingMin.GetFormatedValue(_savingsProduct.UseCents);
            tbInitialAmountMax.Text = _savingsProduct.InitialAmountMax.GetFormatedValue(_savingsProduct.UseCents);
            tbInitialAmountMin.Text = _savingsProduct.InitialAmountMin.GetFormatedValue(_savingsProduct.UseCents);
            tbTransferMin.Text = _savingsProduct.TransferMin.GetFormatedValue(_savingsProduct.UseCents);
            tbTransferMax.Text = _savingsProduct.TransferMax.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.InterestRateMax.HasValue) tbInterestRateMax.Text = (_savingsProduct.InterestRateMax.Value * 100).ToString();
            if (_savingsProduct.InterestRateMin.HasValue) tbInterestRateMin.Text = (_savingsProduct.InterestRateMin.Value * 100).ToString();
            if (_savingsProduct.InterestRate.HasValue) tbInterestRateValue.Text = (_savingsProduct.InterestRate.Value * 100).ToString();
            cbPosting.SelectedValue = _savingsProduct.InterestFrequency.ToString();
            cbAccrual.Text = _savingsProduct.InterestBase.ToString();
            tbDepositMin.Text = _savingsProduct.DepositMin.Value.ToString("F");
            tbDepositMax.Text = _savingsProduct.DepositMax.Value.ToString("F");
            tbDepositFeesMin.Text = _savingsProduct.DepositFeesMin.Value.ToString("F");
            tbDepositFeesMax.Text = _savingsProduct.DepositFeesMax.Value.ToString("F");
            tbIntraTransferFeesMin.Text = (_savingsProduct.FlatTransferFeesMin.HasValue && _savingsProduct.FlatTransferFeesMin.Value > 0)
                ? _savingsProduct.FlatTransferFeesMin.GetFormatedValue(_savingsProduct.UseCents)
                : (_savingsProduct.RateTransferFeesMin.Value * 100).ToString(CultureInfo.InvariantCulture);
            tbIntraTransferFeesMax.Text = (_savingsProduct.FlatTransferFeesMax.HasValue && _savingsProduct.FlatTransferFeesMax.Value > 0)
                ? _savingsProduct.FlatTransferFeesMax.GetFormatedValue(_savingsProduct.UseCents)
                : (_savingsProduct.RateTransferFeesMax.Value * 100).ToString(CultureInfo.InvariantCulture);
            if (_savingsProduct.InterBranchTransferFee.Min != null) tbInterTransferFeesMin.Text = _savingsProduct.InterBranchTransferFee.Min.Value.ToString("F");
            if (_savingsProduct.InterBranchTransferFee.Max != null) tbInterTransferFeesMax.Text = _savingsProduct.InterBranchTransferFee.Max.Value.ToString("F");
            if (_savingsProduct.InterBranchTransferFee.IsFlat)
                rbInterFlatTransferFees.Checked = true;
            else
                rbInterRateTransferFees.Checked = true;
            RadioButtonChanged();
            rbIntraFlatTransferFees.Checked = _savingsProduct.TransferFeesType == OSavingsFeesType.Flat;
            rbFlatWithdrawFees.CheckedChanged += (sender, e) => RadioButtonChanged();
            rbRateWithdrawFees.CheckedChanged += (sender, e) => RadioButtonChanged();
            rbIntraFlatTransferFees.CheckedChanged += (sender, e) => RadioButtonChanged();
            rbIntraRateTransferFees.CheckedChanged += (sender, e) => RadioButtonChanged();
            rbInterFlatTransferFees.CheckedChanged += (sender, e) => RadioButtonChanged();
            rbInterRateTransferFees.CheckedChanged += (sender, e) => RadioButtonChanged();
            rbFlatDepositFees.CheckedChanged += (sender, e) => RadioButtonChanged();
            rbRateDepositFees.CheckedChanged += (sender, e) => RadioButtonChanged();
            if (_savingsProduct.InterestBase == OSavingInterestBase.Monthly || _savingsProduct.InterestBase == OSavingInterestBase.Weekly)
                cbCalculAmount.SelectedValue = _savingsProduct.CalculAmountBase.ToString();

            if (_savingsProduct.EntryFees.HasValue) tbEntryFeesValue.Text = _savingsProduct.EntryFees.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.EntryFeesMin.HasValue) tbEntryFeesMin.Text = _savingsProduct.EntryFeesMin.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.EntryFeesMax.HasValue) tbEntryFeesMax.Text = _savingsProduct.EntryFeesMax.GetFormatedValue(_savingsProduct.UseCents);

            if (_savingsProduct.WithdrawFeesType == OSavingsFeesType.Flat)
            {
                if (_savingsProduct.FlatWithdrawFees.HasValue) 
                    tbWithdrawFees.Text = _savingsProduct.FlatWithdrawFees.GetFormatedValue(_savingsProduct.UseCents);
                if (_savingsProduct.FlatWithdrawFeesMin.HasValue) 
                    tbWithdrawFeesMin.Text = _savingsProduct.FlatWithdrawFeesMin.GetFormatedValue(_savingsProduct.UseCents);
                if (_savingsProduct.FlatWithdrawFeesMax.HasValue) 
                    tbWithdrawFeesMax.Text = _savingsProduct.FlatWithdrawFeesMax.GetFormatedValue(_savingsProduct.UseCents);
                rbFlatWithdrawFees.Checked = true;
            }
            else
            {
                if (_savingsProduct.RateWithdrawFees.HasValue) 
                    tbWithdrawFees.Text = (_savingsProduct.RateWithdrawFees.Value * 100).ToString();
                if (_savingsProduct.RateWithdrawFeesMin.HasValue) 
                    tbWithdrawFeesMin.Text = (_savingsProduct.RateWithdrawFeesMin.Value * 100).ToString();
                if (_savingsProduct.RateWithdrawFeesMax.HasValue) 
                    tbWithdrawFeesMax.Text = (_savingsProduct.RateWithdrawFeesMax.Value * 100).ToString();
                rbRateWithdrawFees.Checked = true;
            }

            if (_savingsProduct.TransferFeesType == OSavingsFeesType.Flat)
            {
                if (_savingsProduct.FlatTransferFees.HasValue) 
                    tbIntraTransferFees.Text = _savingsProduct.FlatTransferFees.GetFormatedValue(_savingsProduct.UseCents);
                if (_savingsProduct.FlatTransferFeesMin.HasValue) 
                    tbIntraTransferFeesMin.Text = _savingsProduct.FlatTransferFeesMin.GetFormatedValue(_savingsProduct.UseCents);
                if (_savingsProduct.FlatTransferFeesMax.HasValue) 
                    tbIntraTransferFeesMax.Text = _savingsProduct.FlatTransferFeesMax.GetFormatedValue(_savingsProduct.UseCents);
            }
            else
            {
                if (_savingsProduct.RateTransferFees.HasValue) 
                    tbIntraTransferFees.Text = (_savingsProduct.RateTransferFees.Value * 100).ToString();
                if (_savingsProduct.RateTransferFeesMin.HasValue) 
                    tbIntraTransferFeesMin.Text = (_savingsProduct.RateTransferFeesMin.Value * 100).ToString();
                if (_savingsProduct.RateTransferFeesMax.HasValue) 
                    tbIntraTransferFeesMax.Text = (_savingsProduct.RateTransferFeesMax.Value * 100).ToString();
                rbIntraRateTransferFees.Checked = true;
            }

            if (ServicesProvider.GetInstance().GetSavingProductServices().IsThisProductAlreadyUsed(product.Id))
            {
                tbName.Enabled = false;
                tbCodeSavingProduct.Enabled = false;
                cbCurrency.Enabled = false;
                gbFrequency.Enabled = false;
                rbFlatWithdrawFees.Enabled = false;
                rbIntraRateTransferFees.Enabled = false;
                rbRateWithdrawFees.Enabled = false;
            }

            if (_savingsProduct.CloseFees.HasValue)
                tbCloseFees.Text = _savingsProduct.CloseFees.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.CloseFeesMin.HasValue)
                tbCloseFeesMin.Text = _savingsProduct.CloseFeesMin.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.CloseFeesMax.HasValue)
                tbCloseFeesMax.Text = _savingsProduct.CloseFeesMax.GetFormatedValue(_savingsProduct.UseCents);

            if (_savingsProduct.ManagementFees.HasValue)
                tbManagementFees.Text = _savingsProduct.ManagementFees.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.ManagementFeesMin.HasValue)
                tbManagementFeesMin.Text = _savingsProduct.ManagementFeesMin.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.ManagementFeesMax.HasValue)
                tbManagementFeesMax.Text = _savingsProduct.ManagementFeesMax.GetFormatedValue(_savingsProduct.UseCents);

            if (_savingsProduct.OverdraftFees.HasValue)
                tbOverdraftFees.Text = _savingsProduct.OverdraftFees.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.OverdraftFeesMin.HasValue)
                tbOverdraftFeesMin.Text = _savingsProduct.OverdraftFeesMin.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.OverdraftFeesMax.HasValue)
                tbOverdraftFeesMax.Text = _savingsProduct.OverdraftFeesMax.GetFormatedValue(_savingsProduct.UseCents);

            if (_savingsProduct.AgioFees.HasValue)
                tbAgioFees.Text = (_savingsProduct.AgioFees.Value * 100).ToString();
            if (_savingsProduct.AgioFeesMin.HasValue)
                tbAgioFeesMin.Text = (_savingsProduct.AgioFeesMin.Value * 100).ToString();
            if (_savingsProduct.AgioFeesMax.HasValue)
                tbAgioFeesMax.Text = (_savingsProduct.AgioFeesMax.Value * 100).ToString();

            if (_savingsProduct.ReopenFees.HasValue) tbReopenFeesValue.Text = _savingsProduct.ReopenFees.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.ReopenFeesMin.HasValue) tbReopenFeesMin.Text = _savingsProduct.ReopenFeesMin.GetFormatedValue(_savingsProduct.UseCents);
            if (_savingsProduct.ReopenFeesMax.HasValue) tbReopenFeesMax.Text = _savingsProduct.ReopenFeesMax.GetFormatedValue(_savingsProduct.UseCents);
        }

        private void InitializeTabPageTermDeposit()
        {
            checkBoxUseTermDeposit.Checked = _savingsProduct.UseTermDeposit;
            tbTermDepositPeriodMin.Text = _savingsProduct.TermDepositPeriodMin.ToString();
            tbTermDepositPeriodMax.Text = _savingsProduct.TermDepositPeriodMax.ToString();
            if (_savingsProduct.InstallmentTypeId!=null)
            {
                foreach (InstallmentType item in cbxPostingfrequency.Items)
                {
                    if (item.Id != _savingsProduct.InstallmentTypeId) continue;
                    cbxPostingfrequency.SelectedItem = item;
                    break;
                }
            }
        }

        private void LoadPostingFrequency()
        {
            var installmentTypes = ServicesProvider.GetInstance().GetSavingProductServices().GetAllInstallmentTypes();
            foreach (var type in installmentTypes)
            {
                cbxPostingfrequency.Items.Add(type);
            }
        }

        private void InitializeClientTypes()
        {
            _savingsProduct.ProductClientTypes = ServicesProvider.GetInstance().GetSavingProductServices().GetAllClientTypes();
            ServicesProvider.GetInstance().GetSavingProductServices().GetProductAssignedClientTypes(_savingsProduct.ProductClientTypes, _savingsProduct.Id);
            foreach (ProductClientType clientType in _savingsProduct.ProductClientTypes)
            {
                switch (clientType.TypeName)
                {
                    case "All":
                        clientTypeAllCheckBox.Checked = clientType.IsChecked;
                        break;
                    case "Group":
                        clientTypeGroupCheckBox.Checked = clientType.IsChecked;
                        break;
                    case "Individual":
                        clientTypeIndivCheckBox.Checked = clientType.IsChecked;
                        break;
                    case "Corporate":
                        clientTypeCorpCheckBox.Checked = clientType.IsChecked;
                        break;
                    case "Village":
                        clientTypeVillageCheckBox.Checked = clientType.IsChecked;
                        break;
                    default: break;
                }
            }
        }

        private void InitializeComboBoxCurrencies()
        {
            cbCurrency.Items.Clear();
            cbCurrency.Text = MultiLanguageStrings.GetString(Ressource.FrmAddLoanProduct, "Currency.Text");
            List<Currency> currencies = ServicesProvider.GetInstance().GetCurrencyServices().FindAllCurrencies();
            Currency line = new Currency { Name = MultiLanguageStrings.GetString(Ressource.FrmAddLoanProduct, "Currency.Text"), Id = 0 };
            cbCurrency.Items.Add(line);

            foreach (Currency cur in currencies)
            {
                cbCurrency.Items.Add(cur);
            }

            bool oneCurrency = 2 == cbCurrency.Items.Count;
            cbCurrency.SelectedIndex = oneCurrency ? 1 : 0;
            cbCurrency.Enabled = !oneCurrency;
        }

        private void btSavingProduct_Click(object sender, EventArgs e)
        {
            _savingsProduct.Name = tbName.Text;
            _savingsProduct.Code = tbCodeSavingProduct.Text;
            _savingsProduct.InterestBase = (OSavingInterestBase)Enum.Parse(typeof(OSavingInterestBase), cbAccrual.SelectedValue.ToString());
            _savingsProduct.InterestFrequency = (OSavingInterestFrequency)Enum.Parse(typeof(OSavingInterestFrequency), cbPosting.SelectedValue.ToString());
            if (_savingsProduct.InterestBase == OSavingInterestBase.Monthly || _savingsProduct.InterestBase == OSavingInterestBase.Weekly
                || _savingsProduct.InterestBase == OSavingInterestBase.Yearly)
                _savingsProduct.CalculAmountBase = (OSavingCalculAmountBase)Enum.Parse(typeof(OSavingCalculAmountBase), cbCalculAmount.SelectedValue.ToString());

            InstallmentType freq = cbManagementFeeFreq.SelectedItem as InstallmentType;
            Debug.Assert(freq != null, "Mgmg fee frequency cannot be null");
            _savingsProduct.ManagementFeeFreq = freq;

            InstallmentType freqAgio = cbAgioFeesFreq.SelectedItem as InstallmentType;
            Debug.Assert(freqAgio != null, "Agio fees frequency cannot be null!");
            _savingsProduct.AgioFeesFreq = freqAgio;
            _savingsProduct.Currency = cbCurrency.SelectedItem as Currency;
            if(_personalAccountRadioButton.Checked)
                _savingsProduct.Type = OSavingProductType.PersonalAccount;
            else if (_shortTermDepositRadioButton.Checked)
                _savingsProduct.Type = OSavingProductType.ShortTermDeposit;
            else
                _savingsProduct.Type = OSavingProductType.Saving;
            RadioButtonChanged();
            _savingsProduct.FlatWithdrawFeesMin = CheckAmount(tbWithdrawFeesMin, true, false);
            _savingsProduct.FlatWithdrawFeesMax = CheckAmount(tbWithdrawFeesMax, true, false);
            if (_savingsProduct.FlatWithdrawFeesMin == _savingsProduct.FlatWithdrawFeesMax)
                _savingsProduct.FlatWithdrawFees = CheckAmount(tbWithdrawFeesMin, true, false);
            
            try
            {
               if (_savingsProduct.PackageMode==OPackageMode.Edit)
               {
                   if (DialogResult.Yes == MessageBox.Show(MultiLanguageStrings.GetString(Ressource.FrmAddSavingProduct, "messageUpdate.Text"),
                                MultiLanguageStrings.GetString(Ressource.FrmAddSavingProduct, "title.Text"), MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                   {
                       ServicesProvider.GetInstance().GetSavingProductServices().SaveProduct(_savingsProduct, clientTypeCounter, true);
                   }
                   else
                   {
                       ServicesProvider.GetInstance().GetSavingProductServices().SaveProduct(_savingsProduct, clientTypeCounter, false);
                   }
               }
               else
               {
                   ServicesProvider.GetInstance().GetSavingProductServices().SaveProduct(_savingsProduct, clientTypeCounter);
               }
                
               DialogResult = DialogResult.OK;
               Close();
            }
            catch (Exception ex)
            {
                new frmShowError(CustomExceptionHandler.ShowExceptionText(ex)).ShowDialog();
            }
        }

        private void tbInitialAmountMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.InitialAmountMin = CheckAmount(tbInitialAmountMin, true, false);
        }

        private void tbInitialAmountMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.InitialAmountMax = CheckAmount(tbInitialAmountMax, true, false);
        }

        private static decimal? Check(TextBox tb, bool allowNegativeNumber)
        {
            tb.BackColor = Color.White;
            try
            {
                if (string.IsNullOrEmpty(tb.Text.Trim())) 
                    return null;

                if (!allowNegativeNumber && Convert.ToDecimal(tb.Text) < 0)
                    throw new Exception();

                if (Convert.ToDecimal(tb.Text) > 999999999999999)
                    throw new Exception();

                return Convert.ToDecimal(tb.Text);
            }
            catch (Exception)
            {
                tb.BackColor = Color.Red;
                return null;
            }
        }

        private static OCurrency CheckAmount(TextBox textBox, bool useOccurrency, bool allowNegativeNumber)
        {
            textBox.BackColor = Color.White;
            try
            {
                if (String.IsNullOrEmpty(textBox.Text.Trim()))
                    return null;
                if (!allowNegativeNumber && Convert.ToDecimal(textBox.Text)<0)
                    throw new Exception();
                
                if (Convert.ToDecimal(textBox.Text) > 999999999999999)
                    throw new Exception();

                return Convert.ToDecimal(textBox.Text);
            }
            catch (Exception)
            {
                textBox.BackColor = Color.Red;
                return null;
            }
        }

        private static double? CheckAmount(TextBox textBox, bool allowNegativeNumber)
        {
            textBox.BackColor = Color.White;
            try
            {
                if (String.IsNullOrEmpty(textBox.Text.Trim()))
                    return  null;
                if (!allowNegativeNumber && Convert.ToDouble(textBox.Text) < 0)
                    throw new Exception();

                if (Convert.ToDouble(textBox.Text) > 999999999999999.0)
                    throw new Exception();
                
                return Convert.ToDouble(textBox.Text) / 100;
            }
            catch (Exception)
            {
                textBox.BackColor = Color.Red;
                return null;
            }
        }

        private void tbBalanceMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.BalanceMin = CheckAmount(tbBalanceMin, true, true);
        }

        private void tbBalanceMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.BalanceMax = CheckAmount(tbBalanceMax, true,false);
        }

        private void tbWithDrawingMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.WithdrawingMin = CheckAmount(tbWithDrawingMin, true, false);
        }

        private void tbDrawingMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.WithdrawingMax = CheckAmount(tbDrawingMax, true, false);
        }

        private static double? GetYearlyInterestRate(double? interestRate, OSavingInterestBase oSavingInterestBase)
        {
            switch (oSavingInterestBase)
            {
                case OSavingInterestBase.Daily: return interestRate * 100 * 365;
                case OSavingInterestBase.Weekly: return interestRate * 100 * 52; 
                case OSavingInterestBase.Monthly: return interestRate * 100 * 12;
                case OSavingInterestBase.Yearly: return interestRate * 100;
                default: return null;
            }
        }

        private void tbInterestRateMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.InterestRateMin = CheckAmount(tbInterestRateMin, false);
            if (_savingsProduct.InterestRateMin == _savingsProduct.InterestRateMax)
            {
                _savingsProduct.InterestRateMin = _savingsProduct.InterestRateMax = null;
                _savingsProduct.InterestRate = CheckAmount(tbInterestRateMin, false);
            }
            lbYearlyInterestRateMin.Text = string.Format("{0} % {1}", GetYearlyInterestRate(_savingsProduct.InterestRateMin == null ? _savingsProduct.InterestRate : _savingsProduct.InterestRateMin, 
                (OSavingInterestBase)Enum.Parse(typeof(OSavingInterestBase), cbAccrual.SelectedValue.ToString())), 
                MultiLanguageStrings.GetString(Ressource.FrmAddSavingProduct, "Yearly.Text"));
        }

        private void tbInterestRateMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.InterestRateMax = CheckAmount(tbInterestRateMax, false);
            if (_savingsProduct.InterestRateMin == _savingsProduct.InterestRateMax)
            {
                _savingsProduct.InterestRateMin = _savingsProduct.InterestRateMax = null;
                _savingsProduct.InterestRate = CheckAmount(tbInterestRateMax, false);
            }
            lbYearlyInterestRateMax.Text = string.Format("{0} % {1}", GetYearlyInterestRate(_savingsProduct.InterestRateMax == null ? _savingsProduct.InterestRate : _savingsProduct.InterestRateMax, 
                (OSavingInterestBase)Enum.Parse(typeof(OSavingInterestBase), cbAccrual.SelectedValue.ToString())), 
                MultiLanguageStrings.GetString(Ressource.FrmAddSavingProduct, "Yearly.Text"));
        }

        private void tbInterestRateValue_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.InterestRate = CheckAmount(tbInterestRateValue, false);
            lbYearlyInterestRate.Text = string.Format("{0} % {1}", GetYearlyInterestRate(_savingsProduct.InterestRate, 
                (OSavingInterestBase)Enum.Parse(typeof(OSavingInterestBase), cbAccrual.SelectedValue.ToString())), 
                MultiLanguageStrings.GetString(Ressource.FrmAddSavingProduct, "Yearly.Text"));
        }

        private void tbDepositMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.DepositMin = CheckAmount(tbDepositMin, true, false);
        }

        private void tbDepositMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.DepositMax = CheckAmount(tbDepositMax, true, false);
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadInterestComboBox()
        {
            cbAccrual.DataBind(typeof(OSavingInterestBase), Ressource.FrmAddSavingProduct, false);
            cbPosting.DataBind(typeof(OSavingInterestFrequency), Ressource.FrmAddSavingProduct, false);
            cbCalculAmount.DataBind(typeof(OSavingCalculAmountBase), Ressource.FrmAddSavingProduct, true);
        }

        private void cbAccrual_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbCalculAmount.Enabled = ((OSavingInterestBase)Enum.Parse(typeof(OSavingInterestBase), 
                cbAccrual.SelectedValue.ToString())) != OSavingInterestBase.Daily;
            lbYearlyInterestRateMin.Text = string.Format("{0} % {1}", GetYearlyInterestRate(_savingsProduct.InterestRateMin == null ? _savingsProduct.InterestRate : _savingsProduct.InterestRateMin, 
                (OSavingInterestBase)Enum.Parse(typeof(OSavingInterestBase), cbAccrual.SelectedValue.ToString())), 
                MultiLanguageStrings.GetString(Ressource.FrmAddSavingProduct, "Yearly.Text"));
            lbYearlyInterestRateMax.Text = string.Format("{0} % {1}", GetYearlyInterestRate(_savingsProduct.InterestRateMax == null ? _savingsProduct.InterestRate : _savingsProduct.InterestRateMax, 
                (OSavingInterestBase)Enum.Parse(typeof(OSavingInterestBase), cbAccrual.SelectedValue.ToString())), 
                MultiLanguageStrings.GetString(Ressource.FrmAddSavingProduct, "Yearly.Text"));
            lbYearlyInterestRate.Text = string.Format("{0} % {1}", GetYearlyInterestRate(_savingsProduct.InterestRate, 
                (OSavingInterestBase)Enum.Parse(typeof(OSavingInterestBase), cbAccrual.SelectedValue.ToString())), 
                MultiLanguageStrings.GetString(Ressource.FrmAddSavingProduct, "Yearly.Text"));
        }

        private void tbEntryFeesMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.EntryFeesMin = CheckAmount(tbEntryFeesMin, true, false);
        }

        private void tbEntryFeesMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.EntryFeesMax = CheckAmount(tbEntryFeesMax, true, false);
        }

        private void tbEntryFeesValue_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.EntryFees = CheckAmount(tbEntryFeesValue, true, false);
        }

        private void FrmAddSavingBookProduct_Load(object sender, EventArgs e)
        {
            tbName.Select();
            _cash = MultiLanguageStrings.GetString(Ressource.FrmAddSavingProduct, "cash");
            _cheque = MultiLanguageStrings.GetString(Ressource.FrmAddSavingProduct, "cheque");
            cbTransactionIn.Items.Add(_cash);
            cbTransactionIn.Items.Add(_cheque);
            cbTransactionIn.SelectedIndex = 0;

            cbIntraTransferType.Items.Add(GetString("FrmAddSavingProduct", "normalTransfers"));
            cbIntraTransferType.Items.Add(GetString("FrmAddSavingProduct", "interBranchTransfers"));
            cbIntraTransferType.SelectedIndex = 0;
        }

        private void tbTransferMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.TransferMin = CheckAmount(tbTransferMin, true, false);
        }

        private void tbTransferMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.TransferMax = CheckAmount(tbTransferMax, true, false);
        }

        private void rbFlatWithdrawFees_CheckedChanged(object sender, EventArgs e)
        {
            _savingsProduct.FlatWithdrawFees = CheckAmount(tbWithdrawFees, true, false);
            _savingsProduct.FlatWithdrawFeesMin = CheckAmount(tbWithdrawFeesMin, true, false);
            _savingsProduct.FlatWithdrawFeesMax = CheckAmount(tbWithdrawFeesMax, true, false);
        }

        private void tbDepositFeesMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.DepositFeesMax = CheckAmount(tbDepositFeesMax, true, false);
        }

        private void tbDepositFeesMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.DepositFeesMin = CheckAmount(tbDepositFeesMin, true, false);
        }

        private void tbCloseFeesMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.CloseFeesMin = CheckAmount(tbCloseFeesMin, true, false);
        }

        private void tbCloseFeesMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.CloseFeesMax = CheckAmount(tbCloseFeesMax, true, false);
        }

        private void tbCloseFees_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.CloseFees = CheckAmount(tbCloseFees, true, false);
        }

        private void tbManagementFeesMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.ManagementFeesMin = CheckAmount(tbManagementFeesMin, true, false);
        }

        private void tbManagementFeesMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.ManagementFeesMax = CheckAmount(tbManagementFeesMax, true, false);
        }

        private void tbManagementFees_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.ManagementFees = CheckAmount(tbManagementFees, true, false);
        }

        private void LoadManagementFeeFreq()
        {
            Debug.Assert(_savingsProduct != null, "Saving product not set");
            ProductServices ps = ServicesProvider.GetInstance().GetProductServices();
            List<InstallmentType> freqs = ps.FindAllInstallmentTypes();
            if (null == freqs) return;
            if (0 == freqs.Count) return;

            string selected = "Monthly";
            cbManagementFeeFreq.Items.Clear();
            foreach (InstallmentType freq in freqs)
            {
                cbManagementFeeFreq.Items.Add(freq);
                if (null == _savingsProduct.ManagementFeeFreq) continue;
                if (_savingsProduct.ManagementFeeFreq.Equals(freq))
                    selected = freq.ToString();
            }

            int index = cbManagementFeeFreq.FindString(selected);
            index = -1 == index ? 0 : index;
            cbManagementFeeFreq.SelectedIndex = index;
        }

        private void LoadAgioFeesFrequency()
        {
            Debug.Assert(_savingsProduct != null, "Saving product not set");
            ProductServices ps = ServicesProvider.GetInstance().GetProductServices();
            List<InstallmentType> freqs = ps.FindAllInstallmentTypes();
            if (null == freqs) return;
            if (0 == freqs.Count) return;

            string selected = "Daily";
            cbAgioFeesFreq.Items.Clear();
            foreach (InstallmentType freq in freqs)
            {
                cbAgioFeesFreq.Items.Add(freq);
                if (null == _savingsProduct.AgioFeesFreq) continue;
                if (_savingsProduct.AgioFeesFreq.Equals(freq))
                    selected = freq.ToString();
            }

            int index = cbAgioFeesFreq.FindString(selected);
            index = -1 == index ? 0 : index;
            cbAgioFeesFreq.SelectedIndex = index;
        }

        private void clientTypeAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AssignClientTypeToProduct(sender);
        }

        private void clientTypeAllCheckBox_Click(object sender, EventArgs e)
        {
            if (clientTypeAllCheckBox.Checked)
            {
                clientTypeIndivCheckBox.Checked = true;
                clientTypeGroupCheckBox.Checked = true;
                clientTypeCorpCheckBox.Checked = true;
                clientTypeVillageCheckBox.Checked = true;
            }
            else
            {
                clientTypeIndivCheckBox.Checked = false;
                clientTypeGroupCheckBox.Checked = false;
                clientTypeCorpCheckBox.Checked = false;
                clientTypeVillageCheckBox.Checked = false;
            }
        }

        private void AssignClientTypeToProduct(object sender)
        {
            CheckBox receivedCheckBox = (CheckBox)sender;
            string clientTypeName;
            switch (receivedCheckBox.Name)
            {
                case "clientTypeAllCheckBox":
                    clientTypeName = "All";
                    break;
                case "clientTypeGroupCheckBox":
                    clientTypeName = "Group";
                    break;
                case "clientTypeIndivCheckBox":
                    clientTypeName = "Individual";
                    break;
                case "clientTypeCorpCheckBox":
                    clientTypeName = "Corporate";
                    break;
                case "clientTypeVillageCheckBox":
                    clientTypeName = "Village";
                    break;
                default:
                    clientTypeName = null;
                    break;
            }

            for (int i = 0; i < _savingsProduct.ProductClientTypes.Count; i++)
            {
                if (_savingsProduct.ProductClientTypes[i].TypeName == clientTypeName)
                {
                    _savingsProduct.ProductClientTypes[i].IsChecked = receivedCheckBox.Checked;
                    break;
                }
            }
        }

        private void clientTypeGroupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AssignClientTypeToProduct(sender);
            if (((CheckBox)sender).Checked)
                clientTypeCounter++;
            else
            {
                clientTypeCounter--;
                clientTypeAllCheckBox.Checked = false;
            }
            if (clientTypeCounter == 4)
                clientTypeAllCheckBox.Checked = true;
            else
            {
                clientTypeAllCheckBox.Checked = false;
            }
        }

        private void tbOverdraftFeesMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.OverdraftFeesMin = CheckAmount(tbOverdraftFeesMin, true, false);
        }

        private void tbOverdraftFeesMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.OverdraftFeesMax = CheckAmount(tbOverdraftFeesMax, true, false);
        }

        private void tbOverdraftFees_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.OverdraftFees = CheckAmount(tbOverdraftFees, true, false);
        }

        private void tbAgioFeesMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.AgioFeesMin = CheckAmount(tbAgioFeesMin, false);
        }

        private void tbAgioFeesMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.AgioFeesMax = CheckAmount(tbAgioFeesMax, false);
        }

        private void tbAgioFees_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.AgioFees = CheckAmount(tbAgioFees, false);
        }

        private void tbReopenFeesMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.ReopenFeesMin = CheckAmount(tbReopenFeesMin, true, false);
        }

        private void tbReopenFeesMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.ReopenFeesMax = CheckAmount(tbReopenFeesMax, true, false);
        }

        private void tbReopenFeesValue_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.ReopenFees = CheckAmount(tbReopenFeesValue, true, false);
        }

        private bool IsInterBranchTransferFees()
        {
            return 1 == cbIntraTransferType.SelectedIndex;
        }

        private void checkBoxUseTermDeposit_CheckedChanged(object sender, EventArgs e)
        {
            termDepositPanel.Enabled = checkBoxUseTermDeposit.Checked;
            _savingsProduct.UseTermDeposit = checkBoxUseTermDeposit.Checked;
        }

        private void tbTermDepositPeriodMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.TermDepositPeriodMin = CheckInt((TextBox) sender);
        }

        private void tbTermDepositPeriodMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.TermDepositPeriodMax = CheckInt((TextBox) sender);
        }

        private static int? CheckInt(TextBox textBox)
        {
            textBox.BackColor = Color.White;
            try
            {
                return Convert.ToInt32(textBox.Text);
            }
            catch (Exception)
            {
                textBox.BackColor = Color.Red;
                return null;
            }
        }

        private void cbxPostingfrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPostingfrequency.SelectedItem!=null)
            {
                _savingsProduct.Periodicity =  (InstallmentType) cbxPostingfrequency.SelectedItem;
            }
        }

        private void _personalAccountRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            gbInitialAmount.Visible = gbInterestRate.Visible = gbFrequency.Visible = gbBalance.Visible = false;
            if(tabControlSaving.TabPages.Contains(tabPageOverdraft))
                tabControlSaving.TabPages.Remove(tabPageOverdraft);
        }

        private void _shortTermDepositRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            gbInitialAmount.Visible = gbInterestRate.Visible = gbFrequency.Visible = gbBalance.Visible = true;
            if (!tabControlSaving.TabPages.Contains(tabPageOverdraft))
                tabControlSaving.TabPages.Add(tabPageOverdraft);
        }

        private void _savingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            gbInitialAmount.Visible = gbInterestRate.Visible = gbFrequency.Visible = gbBalance.Visible = true;
            if (!tabControlSaving.TabPages.Contains(tabPageOverdraft))
                tabControlSaving.TabPages.Add(tabPageOverdraft);
        }

        private void tbWithdrawFeesMin_TextChanged_1(object sender, EventArgs e)
        {
            if (rbFlatWithdrawFees.Checked)
            {
                _savingsProduct.FlatWithdrawFeesMin = CheckAmount(tbWithdrawFeesMin, true, false);
                if (_savingsProduct.FlatWithdrawFeesMin == _savingsProduct.FlatWithdrawFeesMax)
                {
                    _savingsProduct.FlatWithdrawFeesMin = _savingsProduct.FlatWithdrawFeesMax = null;
                    _savingsProduct.FlatWithdrawFees = CheckAmount(tbWithdrawFeesMin, true, false);
                }
            }
            if (rbRateWithdrawFees.Checked)
            {
                _savingsProduct.RateWithdrawFeesMin = CheckAmount(tbWithdrawFeesMin, false);
                if (_savingsProduct.RateWithdrawFeesMin == _savingsProduct.RateWithdrawFeesMax)
                {
                    _savingsProduct.RateWithdrawFeesMin = _savingsProduct.RateWithdrawFeesMax = null;
                    _savingsProduct.RateWithdrawFees = CheckAmount(tbWithdrawFeesMin, false);
                }
            }
        }

        private void tbWithdrawFeesMax_TextChanged_1(object sender, EventArgs e)
        {
            if (rbFlatWithdrawFees.Checked)
            {
                _savingsProduct.FlatWithdrawFeesMax = CheckAmount(tbWithdrawFeesMax, true, false);
                if (_savingsProduct.FlatWithdrawFeesMin == _savingsProduct.FlatWithdrawFeesMax)
                {
                    _savingsProduct.FlatWithdrawFeesMin = _savingsProduct.FlatWithdrawFeesMax = null;
                    _savingsProduct.FlatWithdrawFees = CheckAmount(tbWithdrawFeesMax, true, false);
                }
            }
            if (rbRateWithdrawFees.Checked)
            {
                _savingsProduct.RateWithdrawFeesMax = CheckAmount(tbWithdrawFeesMax, false);
                if (_savingsProduct.RateWithdrawFeesMin == _savingsProduct.RateWithdrawFeesMax)
                {
                    _savingsProduct.RateWithdrawFeesMin = _savingsProduct.RateWithdrawFeesMax = null;
                    _savingsProduct.RateWithdrawFees = CheckAmount(tbWithdrawFeesMax, false);
                }
            }
        }

        private void tbIntraTransferFeesMin_TextChanged(object sender, EventArgs e)
        {
            if (_savingsProduct.TransferFeesType == OSavingsFeesType.Flat)
                _savingsProduct.FlatTransferFeesMin = CheckAmount(tbIntraTransferFeesMin, true, false);
            if (_savingsProduct.TransferFeesType == OSavingsFeesType.Rate)
                _savingsProduct.RateTransferFeesMin = CheckAmount(tbIntraTransferFeesMin, false);
        }

        private void tbIntraTransferFeesMax_TextChanged(object sender, EventArgs e)
        {
            if (_savingsProduct.TransferFeesType == OSavingsFeesType.Flat)
                _savingsProduct.FlatTransferFeesMax = CheckAmount(tbIntraTransferFeesMax, true, false);
            if (_savingsProduct.TransferFeesType == OSavingsFeesType.Rate)
                _savingsProduct.FlatTransferFeesMax = CheckAmount(tbIntraTransferFeesMax, true, false);
        }

        private void tbInterTransferFeesMin_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.InterBranchTransferFee.Min = Check(tbInterTransferFeesMin, false);
        }

        private void tbInterTransferFeesMax_TextChanged(object sender, EventArgs e)
        {
            _savingsProduct.InterBranchTransferFee.Max = Check(tbInterTransferFeesMax, false);
        }

        void RadioButtonChanged()
        {
            _savingsProduct.WithdrawFeesType = rbFlatWithdrawFees.Checked ? OSavingsFeesType.Flat : OSavingsFeesType.Rate;
            if (rbFlatWithdrawFees.Checked)
            {
                _savingsProduct.FlatWithdrawFeesMin = CheckAmount(tbWithdrawFeesMin, true, false);
                _savingsProduct.FlatWithdrawFeesMax = CheckAmount(tbWithdrawFeesMax, true, false);
                if (_savingsProduct.FlatWithdrawFeesMin == _savingsProduct.FlatWithdrawFeesMax)
                {
                    _savingsProduct.FlatWithdrawFeesMin = _savingsProduct.FlatWithdrawFeesMax = null;
                    _savingsProduct.FlatWithdrawFees = CheckAmount(tbWithdrawFeesMin, true, false);
                }
            }
            if (rbRateWithdrawFees.Checked)
            {
                _savingsProduct.RateWithdrawFeesMin = CheckAmount(tbWithdrawFeesMin, false);
                _savingsProduct.RateWithdrawFeesMax = CheckAmount(tbWithdrawFeesMax, false);
                if (_savingsProduct.RateWithdrawFeesMin == _savingsProduct.RateWithdrawFeesMax)
                {
                    _savingsProduct.RateWithdrawFeesMin = _savingsProduct.RateWithdrawFeesMax = null;
                    _savingsProduct.RateWithdrawFees = CheckAmount(tbWithdrawFeesMin, false);
                }
            }
            _savingsProduct.TransferFeesType = rbIntraFlatTransferFees.Checked? OSavingsFeesType.Flat: OSavingsFeesType.Rate;
            if (_savingsProduct.TransferFeesType == OSavingsFeesType.Rate)
            {
                _savingsProduct.RateTransferFeesMin = CheckAmount(tbIntraTransferFeesMin, false);
                _savingsProduct.RateTransferFeesMax = CheckAmount(tbIntraTransferFeesMax, false);
                _savingsProduct.FlatTransferFeesMin = _savingsProduct.FlatTransferFeesMax = null;
            }
            if (_savingsProduct.TransferFeesType == OSavingsFeesType.Flat)
            {
                _savingsProduct.FlatTransferFeesMin = CheckAmount(tbIntraTransferFeesMin, true, false);
                _savingsProduct.FlatTransferFeesMax = CheckAmount(tbIntraTransferFeesMax, true, false);
                _savingsProduct.RateTransferFeesMin = _savingsProduct.RateTransferFeesMax = null;
            }
            _savingsProduct.InterBranchTransferFee.IsFlat = rbInterFlatTransferFees.Checked;

            _savingsProduct.AgioFeesMin = CheckAmount(tbAgioFeesMin, false);
            _savingsProduct.AgioFeesMax = CheckAmount(tbAgioFeesMax, false);
            if (_savingsProduct.AgioFeesMin == _savingsProduct.AgioFeesMax)
            {
                _savingsProduct.AgioFeesMin = _savingsProduct.AgioFeesMax = null;
                _savingsProduct.AgioFees = CheckAmount(tbAgioFeesMin, false);
            }
        }
    }
}
