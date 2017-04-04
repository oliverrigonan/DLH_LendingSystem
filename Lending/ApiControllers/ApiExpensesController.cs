using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Lending.ApiControllers
{
    public class ApiExpensesController : ApiController
    {
        // data
        private Data.LendingDataContext db = new Data.LendingDataContext();

        // expenses list
        [Authorize]
        [HttpGet]
        [Route("api/expenses/listByExpensesDate/{startExpenseDate}/{endExpensesData}")]
        public List<Models.TrnExpenses> listExpenseByExpensesDate(String startExpenseDate, String endExpensesData)
        {
            var expenses = from d in db.trnExpenses.OrderByDescending(d => d.Id)
                           where d.ExpenseDate >= Convert.ToDateTime(startExpenseDate)
                           && d.ExpenseDate <= Convert.ToDateTime(endExpensesData)
                           select new Models.TrnExpenses
                           {
                               Id = d.Id,
                               ExpenseNumber = d.ExpenseNumber,
                               ExpenseDate = d.ExpenseDate.ToShortDateString(),
                               AssignedStaffId = d.AssignedStaffId,
                               AssignedStaff = d.mstStaff.Staff,
                               Particulars = d.Particulars,
                               IsCollectionExpense = d.IsCollectionExpense,
                               ExpenseAmount = d.ExpenseAmount,
                               PreparedByUserId = d.PreparedByUserId,
                               PreparedByUser = d.mstUser.FullName,
                               IsLocked = d.IsLocked,
                               CreatedByUserId = d.CreatedByUserId,
                               CreatedByUser = d.mstUser1.FullName,
                               CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                               UpdatedByUserId = d.UpdatedByUserId,
                               UpdatedByUser = d.mstUser2.FullName,
                               UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                           };

            return expenses.ToList();
        }

        // expenses by id
        [Authorize]
        [HttpGet]
        [Route("api/expenses/getById/{id}")]
        public Models.TrnExpenses getExpensesById(String id)
        {
            var expense = from d in db.trnExpenses
                          where d.Id == Convert.ToInt32(id)
                          select new Models.TrnExpenses
                          {
                              Id = d.Id,
                              ExpenseNumber = d.ExpenseNumber,
                              ExpenseDate = d.ExpenseDate.ToShortDateString(),
                              AssignedStaffId = d.AssignedStaffId,
                              AssignedStaff = d.mstStaff.Staff,
                              Particulars = d.Particulars,
                              IsCollectionExpense = d.IsCollectionExpense,
                              ExpenseAmount = d.ExpenseAmount,
                              PreparedByUserId = d.PreparedByUserId,
                              PreparedByUser = d.mstUser.FullName,
                              IsLocked = d.IsLocked,
                              CreatedByUserId = d.CreatedByUserId,
                              CreatedByUser = d.mstUser1.FullName,
                              CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                              UpdatedByUserId = d.UpdatedByUserId,
                              UpdatedByUser = d.mstUser2.FullName,
                              UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                          };

            return (Models.TrnExpenses)expense.FirstOrDefault();
        }

        // zero fill
        public String zeroFill(Int32 number, Int32 length)
        {
            var result = number.ToString();
            var pad = length - result.Length;
            while (pad > 0)
            {
                result = "0" + result;
                pad--;
            }

            return result;
        }

        // add expenses
        [Authorize]
        [HttpPost]
        [Route("api/expenses/add")]
        public Int32 addExpenses()
        {
            try
            {
                var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                var mstUserForms = from d in db.mstUserForms
                                   where d.UserId == userId
                                   select new Models.MstUserForm
                                   {
                                       Id = d.Id,
                                       Form = d.sysForm.Form,
                                       CanPerformActions = d.CanPerformActions
                                   };

                if (mstUserForms.Any())
                {
                    String matchPageString = "ExpensesList";
                    Boolean canPerformActions = false;

                    foreach (var mstUserForm in mstUserForms)
                    {
                        if (mstUserForm.Form.Equals(matchPageString))
                        {
                            if (mstUserForm.CanPerformActions)
                            {
                                canPerformActions = true;
                            }

                            break;
                        }
                    }

                    if (canPerformActions)
                    {
                        String expenseNumber = "0000000001";
                        var expense = from d in db.trnExpenses.OrderByDescending(d => d.Id) select d;
                        if (expense.Any())
                        {
                            var newExpenseNumber = Convert.ToInt32(expense.FirstOrDefault().ExpenseNumber) + 0000000001;
                            expenseNumber = newExpenseNumber.ToString();
                        }

                        Data.trnExpense newExpense = new Data.trnExpense();
                        newExpense.ExpenseNumber = zeroFill(Convert.ToInt32(expenseNumber), 10);
                        newExpense.ExpenseDate = DateTime.Today;
                        newExpense.AssignedStaffId = (from d in db.mstStaffs select d.Id).FirstOrDefault();
                        newExpense.Particulars = "NA";
                        newExpense.IsCollectionExpense = false;
                        newExpense.ExpenseAmount = 0;
                        newExpense.PreparedByUserId = userId;
                        newExpense.IsLocked = false;
                        newExpense.CreatedByUserId = userId;
                        newExpense.CreatedDateTime = DateTime.Now;
                        newExpense.UpdatedByUserId = userId;
                        newExpense.UpdatedDateTime = DateTime.Now;

                        db.trnExpenses.InsertOnSubmit(newExpense);
                        db.SubmitChanges();

                        return newExpense.Id;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        // lock expenses
        [Authorize]
        [HttpPut]
        [Route("api/expenses/lock/{id}")]
        public HttpResponseMessage lockExpenses(String id, Models.TrnExpenses expense)
        {
            try
            {
                var expenses = from d in db.trnExpenses where d.Id == Convert.ToInt32(id) select d;
                if (expenses.Any())
                {
                    if (!expenses.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        var mstUserForms = from d in db.mstUserForms
                                           where d.UserId == userId
                                           select new Models.MstUserForm
                                           {
                                               Id = d.Id,
                                               Form = d.sysForm.Form,
                                               CanPerformActions = d.CanPerformActions
                                           };

                        if (mstUserForms.Any())
                        {
                            String matchPageString = "ExpenseDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString))
                                {
                                    if (mstUserForm.CanPerformActions)
                                    {
                                        canPerformActions = true;
                                    }

                                    break;
                                }
                            }

                            if (canPerformActions)
                            {
                                var lockExpense = expenses.FirstOrDefault();
                                lockExpense.ExpenseDate = Convert.ToDateTime(expense.ExpenseDate);
                                lockExpense.AssignedStaffId = expense.AssignedStaffId;
                                lockExpense.Particulars = expense.Particulars;
                                lockExpense.IsCollectionExpense = expense.IsCollectionExpense;
                                lockExpense.ExpenseAmount = expense.ExpenseAmount;
                                lockExpense.PreparedByUserId = expense.PreparedByUserId;
                                lockExpense.IsLocked = true;
                                lockExpense.UpdatedByUserId = userId;
                                lockExpense.UpdatedDateTime = DateTime.Now;

                                db.SubmitChanges();

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // unlock expenses
        [Authorize]
        [HttpPut]
        [Route("api/expenses/unlock/{id}")]
        public HttpResponseMessage unlockExpenses(String id)
        {
            try
            {
                var expenses = from d in db.trnExpenses where d.Id == Convert.ToInt32(id) select d;
                if (expenses.Any())
                {
                    if (expenses.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        var mstUserForms = from d in db.mstUserForms
                                           where d.UserId == userId
                                           select new Models.MstUserForm
                                           {
                                               Id = d.Id,
                                               Form = d.sysForm.Form,
                                               CanPerformActions = d.CanPerformActions
                                           };

                        if (mstUserForms.Any())
                        {
                            String matchPageString = "ExpenseDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString))
                                {
                                    if (mstUserForm.CanPerformActions)
                                    {
                                        canPerformActions = true;
                                    }

                                    break;
                                }
                            }

                            if (canPerformActions)
                            {
                                var unlockExpense = expenses.FirstOrDefault();
                                unlockExpense.IsLocked = false;
                                unlockExpense.UpdatedByUserId = userId;
                                unlockExpense.UpdatedDateTime = DateTime.Now;
                                db.SubmitChanges();

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // delete expenses
        [Authorize]
        [HttpDelete]
        [Route("api/expenses/delete/{id}")]
        public HttpResponseMessage deleteExpenses(String id)
        {
            try
            {
                var expenses = from d in db.trnExpenses where d.Id == Convert.ToInt32(id) select d;
                if (expenses.Any())
                {
                    if (!expenses.FirstOrDefault().IsLocked)
                    {
                        var userId = (from d in db.mstUsers where d.AspUserId == User.Identity.GetUserId() select d.Id).SingleOrDefault();
                        var mstUserForms = from d in db.mstUserForms
                                           where d.UserId == userId
                                           select new Models.MstUserForm
                                           {
                                               Id = d.Id,
                                               Form = d.sysForm.Form,
                                               CanPerformActions = d.CanPerformActions
                                           };

                        if (mstUserForms.Any())
                        {
                            String matchPageString = "ExpenseDetail";
                            Boolean canPerformActions = false;

                            foreach (var mstUserForm in mstUserForms)
                            {
                                if (mstUserForm.Form.Equals(matchPageString))
                                {
                                    if (mstUserForm.CanPerformActions)
                                    {
                                        canPerformActions = true;
                                    }

                                    break;
                                }
                            }

                            if (canPerformActions)
                            {
                                db.trnExpenses.DeleteOnSubmit(expenses.First());
                                db.SubmitChanges();

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // total collection
        [Authorize]
        [HttpGet]
        [Route("api/expenses/totalExpenses/{date}/{staffId}")]
        public Decimal totalExpense(String date, String staffId)
        {
            var expense = from d in db.trnExpenses
                              where d.ExpenseDate == Convert.ToDateTime(date)
                              && d.AssignedStaffId == Convert.ToInt32(staffId)
                              && d.IsCollectionExpense == true
                              && d.IsLocked == true
                              select d;

            Decimal totalExpenseAmount = 0;
            if (expense.Any())
            {
                totalExpenseAmount = expense.Sum(d => d.ExpenseAmount);
            }

            return totalExpenseAmount;
        }
    }
}
