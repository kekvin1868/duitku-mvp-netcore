using DuitkuMvpNetApp.Data;
using DuitkuMvpNetApp.Models;
using DuitkuMvpNetApp.Models.Response;
using DuitkuMvpNetApp.Models.Payment;
using DuitkuMvpNetApp.Factories.Transactions;
using DuitkuMvpNetApp.ControllerModels;
using DuitkuMvpNetApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Features;

namespace DuitkuMvpNetApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TransactionFactory _transactionFactory;

        public TransactionController(ApplicationDbContext context, TransactionFactory transactionFactory)
        {
            _context = context;
            _transactionFactory = transactionFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string newTransactionId;
            bool transactionExists;

            do
            {
                newTransactionId = _transactionFactory.GenerateUniqueTransactionId();

                // Check if a transaction with this ID already exists
                transactionExists = await _context.Transactions
                    .AnyAsync(t => t.MsTransactionId == newTransactionId);

            } while (transactionExists);

            ViewBag.TransactionId = newTransactionId;

            return View("Index");
        }

        [HttpPost("SelectPaymentMethod")]
        public async Task<IActionResult> SelectPaymentMethod([FromForm] string itemsJson, [FromForm] string transactionId)
        {
            if (string.IsNullOrEmpty(itemsJson))
            {
                return BadRequest("Items data is missing.");
            }

            DataItem addedItemList;
            try
            {
                addedItemList = JsonConvert.DeserializeObject<DataItem>(itemsJson) ?? new DataItem();

                if (addedItemList == null || addedItemList.Items == null || !addedItemList.Items.Any())
                {
                    return BadRequest("Deserialized items are null.");
                }
            }
            catch (JsonException ex)
            {
                return BadRequest($"Error deserializing transactions: {ex.Message}");
            }

            var totalAmount = addedItemList.Items.Sum(t => t.MsItemsHarga);
            var merchantCode = "DS19890";
            var apiKey = "6a9b62c66ed1b89c845fa855514bfcae";
            var datetimePayload = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            var hashText = $"{merchantCode}{totalAmount}{datetimePayload}{apiKey}";

            using (var sha256 = SHA256.Create())
            {
                var signatureBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashText));
                var signaturePayload = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                var requestBody = new
                {
                    merchantcode = merchantCode,
                    amount = totalAmount.ToString(),
                    datetime = datetimePayload,
                    signature = signaturePayload
                };

                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var bodyContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                
                using (var client = new HttpClient())
                try
                {
                    HttpResponseMessage res = await client.PostAsJsonAsync(
                        "https://sandbox.duitku.com/webapi/api/merchant/paymentmethod/getpaymentmethod",
                        requestBody
                    );

                    if (res.IsSuccessStatusCode)
                    {
                        var result = await res.Content.ReadAsStringAsync();
                        var deserializedResponse = JsonConvert.DeserializeObject<PaymentResponse>(result);

                        if (deserializedResponse == null)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to deserialize payment response. ");
                        }

                        var paymentPayload = new PaymentControllerModel
                        {
                            PaymentResponse = deserializedResponse,
                            TotalAmount = totalAmount
                        };

                        var newTransaction = new TransactionControllerModel  {
                            TransactionId = transactionId,
                            TotalAmount = totalAmount,
                            Timestamp = DateTime.Now,
                            Description = addedItemList.Description,
                            Items = addedItemList.Items
                        };

                        var combinedModel = new PaymentTransactionCombinedModel
                        {
                            PaymentCombinedModel = paymentPayload,
                            TransactionCombinedModel = newTransaction,
                        };

                        return View("../Payment/PaymentMethodSelection", combinedModel);
                    }
                    else
                    {
                        // Handle errors
                        return StatusCode((int)res.StatusCode, res.ReasonPhrase);
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Request Exception: " + ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Request error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("General Exception: " + ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, $"General error: {ex.Message}");
                }
            }
        }

        [HttpPost("Payment")]
        public async Task<IActionResult> Payment([FromForm] PaymentFormViewModel model)
        { 
            var transactionDataJson = model.TransactionData;

            if (string.IsNullOrWhiteSpace(transactionDataJson))
            {
                return BadRequest("Received JSON is empty or null.");
            }

            TransactionControllerModel currentTransaction;
            try
            {
                currentTransaction = JsonConvert.DeserializeObject<TransactionControllerModel>(transactionDataJson);
            }
            catch (JsonSerializationException ex)
            {
                return BadRequest("Failed to deserialize JSON: " + ex.ToString);
            }

            if (currentTransaction == null)
            {
                return BadRequest("CurrentTransaction is not set.");
            }
            
            var merchantCodePayload = "DS19890";
            var paymentAmountPayload = currentTransaction.TotalAmount;
            var apiKeyPayload = "6a9b62c66ed1b89c845fa855514bfcae";
            var merchantOrderIdPayload = currentTransaction.Items[0].MsItemsTransactionId;
            var productDetails = currentTransaction.Description;
            var emailPayload = "kevin.hartono1868@gmail.com";
            var paymentMethodPayload = model.PaymentMethod;
            var returnUrlPayload = "http://localhost:5134/Menu";
            var callbackUrlPayload = "https://b5d4-182-253-47-123.ngrok-free.app/Transaction/ReceivePost";

            var signatureText = $"{merchantCodePayload}{merchantOrderIdPayload}{paymentAmountPayload}{apiKeyPayload}";
            var signaturePayload = ComputeMd5Hash(signatureText);

            var requestBody = new
            {
                merchantCode = merchantCodePayload,
                paymentAmount = paymentAmountPayload,
                merchantOrderId = merchantOrderIdPayload,
                productDetails = productDetails,
                email = emailPayload,
                paymentMethod = paymentMethodPayload,
                customerVaName = "Kevin Hartono",
                returnUrl = returnUrlPayload,
                callbackUrl = callbackUrlPayload,
                signature = signaturePayload
            };

            var jsonBody = JsonConvert.SerializeObject(requestBody);
            var bodyContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            string contentString = await bodyContent.ReadAsStringAsync();

            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage res = await client.PostAsync(
                        "https://sandbox.duitku.com/webapi/api/merchant/v2/inquiry",
                        bodyContent
                    );

                    if (res.IsSuccessStatusCode)
                    {
                        var result = await res.Content.ReadAsStringAsync();

                        CreateTransactionPayloadModel newTransactionPayload = new CreateTransactionPayloadModel {
                            TransactionId = currentTransaction.TransactionId,
                            TotalAmount = currentTransaction.TotalAmount,
                            Description = currentTransaction.Description
                        };

                        // POST current Transaction
                        var newTransaction = await _transactionFactory
                            .CreateNewTransactionAsync(newTransactionPayload);

                        _context.Transactions.Add(newTransaction);
                        await _context.SaveChangesAsync();

                        // Insert ALL items from current transaction
                        foreach (var item in currentTransaction.Items)
                        {
                            var newItem = new Item
                            {
                                MsItemsNama = item.MsItemsNama,
                                MsItemsHarga = item.MsItemsHarga,
                                MsItemsTransactionId = item.MsItemsTransactionId
                            };
                            _context.Items.Add(newItem);
                        }
                        await _context.SaveChangesAsync();

                        var transactionResponse = JsonConvert.DeserializeObject<GetTransactionResponse>(result);

                        if (transactionResponse == null)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to deserialize payment response. ");
                        }

                        return Redirect(transactionResponse.PaymentUrl);
                    }
                    else
                    {
                        // Handle errors
                        return StatusCode((int)res.StatusCode, res.ReasonPhrase);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Request Exception: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Request error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Exception: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"General error: {ex.Message}");
            }
        }

        [HttpGet("/All")]
        public async Task<IActionResult> All()
        {
            List<Transaction> allTransactions = await _context.Transactions
                .OrderByDescending(t => t.MsTransactionTimestamp)
                .ToListAsync();

            TransactionViewModel viewModel = new TransactionViewModel
            {
                Transactions = allTransactions
            };

            return View(viewModel);
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(string merchantOrderId)
        {
            // Retrieve the transaction from the database
            Transaction? getTransactionDetails = await _context.Transactions
                .FirstOrDefaultAsync(t => t.MsTransactionId == merchantOrderId);

            List<Item> getItemDetails = await _context.Items
                .Where(i => i.MsItemsTransactionId == merchantOrderId)
                .ToListAsync();
            
            if (getTransactionDetails == null && getItemDetails == null)
            {
                return BadRequest(new { message = "Error processing your request." });
            }

            // Create a view model and pass it to the view
            TransactionEditModel viewModel = new TransactionEditModel
            {
                TransactionDetails = getTransactionDetails ?? new Transaction(),
                ItemDetails = getItemDetails ?? new List<Item>()
            };

            return View(viewModel);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromForm] TransactionEditModel model)
        {
            if (model.TransactionDetails == null || string.IsNullOrWhiteSpace(model.TransactionDetails.MsTransactionDescription))
            {
                ModelState.AddModelError("MsTransactionDescription", "Description is required.");
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                return View("Edit", model);
            }

            var updateTransaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.MsTransactionId == model.TransactionDetails.MsTransactionId);

            if (updateTransaction == null)
            {
                return BadRequest(new { message = "Could not retrieve Transaction." });
            }

            updateTransaction.MsTransactionDescription = model.TransactionDetails.MsTransactionDescription;
            await _context.SaveChangesAsync();

            return RedirectToAction("All", "Transaction");
        }

        [HttpPost("ReceivePost")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult ReceivePost([FromForm] CallbackModel test) {
            var res = JsonConvert.SerializeObject(test);
            Console.WriteLine(res);

            return Json(test);
            // try
            // {
            //     using (HttpClient client = new HttpClient())
            //     {
            //         HttpResponseMessage res = await client.PostAsync(
            //             "https://sandbox.duitku.com/webapi/api/merchant/v2/inquiry",
            //             test
            //         );

            //         if (res)
            //         {

            //         }
            //         else
            //         {
            //             return StatusCode((int)res.StatusCode, res.ReasonPhrase);
            //         }
            //     }
            // }
            // catch (HttpRequestException ex)
            // {
            //     Console.WriteLine("Request Exception: " + ex.Message);
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine("General exception: " + ex.Message);
            //     return StatusCode(StatusCodes.Status500InternalServerError, $"General error: {ex.Message}");
            // }
        }
    
        [HttpGet("ReceiveGet")]
        public IActionResult ReceiveGet() {
            return Content("This is a GET call.");
        }

        private static string ComputeMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert bytes to hex string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}