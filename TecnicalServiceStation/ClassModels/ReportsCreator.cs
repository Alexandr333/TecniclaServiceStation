using MyWordLazyWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecnicalServiceStation.ClassModels
{
    class ReportsCreator
    {
        public static void createOrder(int OrderID)
        {
            TSSEntities TSS = new TSSEntities();
            WordDocument document = new WordDocument(Environment.CurrentDirectory + @"/templates/Order.dotx");
            try
            {
                #region calculations
                Order order = TSS.Order.Where(
                    Order => Order.ID == OrderID
                ).FirstOrDefault();
                Organization organization = TSS.Organization.First();
                if (order == null)
                {
                    return;
                }
                double serviceTotalCost = 0, sparePartTotalCost = 0, serviceTotalTime = 0, totalCostNoVAT = 0, sumVAT = 0, totalCost = 0;
                int sparePartTotalNumber = 0, ClientSparePartsTotal = 0;
                string totalCostString = "";
                DateTime planedExecutionDate = DateTime.Now;
                if (order.OrderService != null)
                {
                    foreach (OrderService orderService in order.OrderService)
                    {
                        serviceTotalTime += orderService.Service.PerfomanceTime;
                        serviceTotalCost += orderService.Service.PerfomanceTime * Convert.ToDouble(orderService.Service.ServiceCategory.CostOfHour);
                    }
                }
                if (order.OrderSpareParts != null)
                {
                    foreach (OrderSpareParts orderSparePart in order.OrderSpareParts)
                    {
                        if (orderSparePart.Own == true)
                        {
                            sparePartTotalNumber += orderSparePart.Number;
                            serviceTotalCost += orderSparePart.Number * Convert.ToDouble(orderSparePart.SparePart.UnitPrice);
                        }
                        else
                        {
                            ClientSparePartsTotal += orderSparePart.Number;
                        }
                    }
                }
                totalCostNoVAT = (serviceTotalCost + sparePartTotalCost);
                sumVAT = totalCostNoVAT * organization.VAT / 100;
                totalCost = sumVAT + totalCostNoVAT;
                planedExecutionDate.AddHours(serviceTotalTime/12 * 24 + 12);
                NumberToMoneyStringConvertor convertor = new NumberToMoneyStringConvertor("RUR", "RUS", "TEXT");
                totalCostString = convertor.convertValue(totalCost);
                #region creation tables content
                List<string[]> ServicesForReport = new List<string[]>();
                foreach (var item in order.OrderService.Select((value, i) => new { value, index = i }))
                {
                    ServicesForReport.Add(
                        new string[]{
                        (item.index+1).ToString(),
                        item.value.Service.Title,
                        item.value.Service.PerfomanceTime.ToString(),
                        "",
                        item.value.Service.ServiceCategory.CostOfHour.ToString(),
                        (((double)item.value.Service.ServiceCategory.CostOfHour)*item.value.Service.PerfomanceTime).ToString()
                    }
                    );
                }
                List<string[]> ClientSparePartsForReport = new List<string[]>();
                List<string[]> SparePartsForReport = new List<string[]>();
                foreach (var item in order.OrderSpareParts.Where(orderSparePart => orderSparePart.Own == true).Select((value, i) => new { value, index = i }))
                {
                    SparePartsForReport.Add(
                        new string[]{
                        (item.index+1).ToString(),
                        item.value.SparePart.Title,
                        item.value.SparePart.Unit.Title,
                        item.value.Number.ToString(),
                        item.value.SparePart.UnitPrice.ToString(),
                        (item.value.SparePart.UnitPrice*item.value.Number).ToString()
                    }
                    );
                }
                foreach (var item in order.OrderSpareParts.Where(orderSparePart => orderSparePart.Own == false).Select((value, i) => new { value, index = i }))
                {
                    ClientSparePartsForReport.Add(
                        new string[]{
                        (item.index+1).ToString(),
                        item.value.SparePart.Title,
                        item.value.SparePart.Unit.Title,
                        item.value.Number.ToString(),
                    }
                    );
                }
                #endregion creation tables content

                #endregion calculations

                #region report creation
                document.AddTextInRange(
                    order.ID.ToString(), document.SelectRangeByMark("##Order.Number")
                );
                document.AddTextInRange(
                    order.ReceiptDate.ToShortDateString(), document.SelectRangeByMark("##Order.ReceiptDate")
                );
                document.AddTextInRange(
                    organization.Title, document.SelectRangeByMark("##Organization.Title ")
                );
                document.AddTextInRange(
                    organization.UTN, document.SelectRangeByMark("##Organization.UTN")
                );
                document.AddTextInRange(
                    organization.Address, document.SelectRangeByMark("##Organization.Address")
                );
                document.AddTextInRange(
                    organization.Address, document.SelectRangeByMark("##Organization.Address")
                );
                document.AddTextInRange(
                    organization.Contacts, document.SelectRangeByMark("##Organization.Contacts")
                );
                document.AddTextInRange(
                    organization.CheckingAccount, document.SelectRangeByMark("##Organization.CheckingAccount")
                );
                document.AddTextInRange(
                    organization.Bank, document.SelectRangeByMark("##Organization.Bank")
                );
                document.AddTextInRange(
                    String.Format("{0} {1} {2}", order.Client.Name, order.Client.Surname, order.Client.PatronymicName), document.SelectRangeByMark("##ClientFullName")
                );
                document.AddTextInRange(
                    order.Client.Address, document.SelectRangeByMark("##Client.Address")
                );
                document.AddTextInRange(
                    order.Client.Contacts, document.SelectRangeByMark("##Client.Contacts")
                );
                document.AddTextInRange(
                    order.Car.Owner, document.SelectRangeByMark("##Car.Owner")
                );
                document.AddTextInRange(
                    order.Car.CarModel.CarMark.Title, document.SelectRangeByMark("##Car.CarMark")
                );
                document.AddTextInRange(
                    order.Car.CarModel.Title, document.SelectRangeByMark("##Car.CarModel")
                );
                document.AddTextInRange(
                    order.Car.RegistrationNymber, document.SelectRangeByMark("##Car.RegistrationNumber")
                );
                document.AddTextInRange(
                    order.Car.VIN, document.SelectRangeByMark("##Car.VIN")
                );
                document.AddTextInRange(
                    order.Car.CreationYear.ToString(), document.SelectRangeByMark("##Car.CreationYear")
                );
                document.AddTextInRange(
                    order.CarMileage.ToString(), document.SelectRangeByMark("##CarMileage")
                );
                document.AddTextInRange(
                    serviceTotalCost.ToString(), document.SelectRangeByMark("##ServiceTotalCost")
                );
                document.AddTextInRange(
                    serviceTotalCost.ToString(), document.SelectRangeByMark("##ServiceTotalCost")
                );
                document.AddTextInRange(
                    serviceTotalTime.ToString(), document.SelectRangeByMark("##ServiceTotalTime")
                );
                document.AddTextInRange(
                    sparePartTotalCost.ToString(), document.SelectRangeByMark("##SparePartsTotalCost")
                );
                document.AddTextInRange(
                    sparePartTotalCost.ToString(), document.SelectRangeByMark("##SparePartsTotalCost")
                );
                document.AddTextInRange(
                    sparePartTotalCost.ToString(), document.SelectRangeByMark("##SparePartsTotalNumber")
                );
                document.AddTextInRange(
                    ClientSparePartsTotal.ToString(), document.SelectRangeByMark("##ClientSparePartsTotal")
                );
                document.AddTextInRange(
                    totalCostNoVAT.ToString(), document.SelectRangeByMark("##totalCostNoVAT")
                );
                document.AddTextInRange(
                    organization.VAT.ToString(), document.SelectRangeByMark("##VAT")
                );
                document.AddTextInRange(
                    sumVAT.ToString(), document.SelectRangeByMark("##SumVAT")
                );
                document.AddTextInRange(
                    totalCost.ToString(), document.SelectRangeByMark("##TotalCostNumber")
                );
                document.AddTextInRange(
                    totalCostString, document.SelectRangeByMark("##TotalCostString")
                );
                document.AddTextInRange(
                    planedExecutionDate.ToShortDateString(), document.SelectRangeByMark("##PlanedExecutionDate")
                );
                document.AddTextInRange(
                    String.Format("{0}.{1}.{2}", order.Worker.Name, order.Worker.Surname[0], order.Worker.PatronymicName[0]), document.SelectRangeByMark("##Worker")
                );
                document.AddTextInRange(
                    String.Format("{0}.{1}.{2}", order.Client.Name, order.Client.Surname[0], order.Client.PatronymicName[0]), document.SelectRangeByMark("##ClientShortFIO")
                );

                #endregion report creation
                #region filling tables
                document.MergeTableWithDocTableBetweenHeaderAndFooter(ServicesForReport, 2);
                document.MergeTableWithDocTableBetweenHeaderAndFooter(SparePartsForReport, 3);
                document.MergeTableWithDocTableBetweenHeaderAndFooter(ClientSparePartsForReport, 4);
                #endregion filling tables
                document.Visible = true;
            }
            catch (Exception)
            {
                document.Close();
            }
        }
    }
}
