using Microsoft.AspNetCore.Mvc;
using MessagingApp1.Models;
using MessagingApp1.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp1.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
            if (user != null)
            {
                return RedirectToAction("Messages", "Account", new { userId = user.Id });
            }
            ModelState.AddModelError("", "Неверный логин или пароль");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Messages(int userId, string sender, DateTime? fromDate, DateTime? toDate, string status, string sortOrder)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var messagesQuery = _context.Messages.AsQueryable();

            if (!string.IsNullOrEmpty(sender))
            {
                messagesQuery = messagesQuery.Where(m => m.From == sender);
            }

            if (fromDate.HasValue)
            {
                messagesQuery = messagesQuery.Where(m => m.SentDate >= fromDate);
            }

            if (toDate.HasValue)
            {
                messagesQuery = messagesQuery.Where(m => m.SentDate <= toDate);
            }

            if (!string.IsNullOrEmpty(status))
            {
                messagesQuery = messagesQuery.Where(m => m.Status == status);
            }

            messagesQuery = sortOrder switch
            {
                "date_asc" => messagesQuery.OrderBy(m => m.SentDate),
                "date_desc" => messagesQuery.OrderByDescending(m => m.SentDate),
                _ => messagesQuery
            };

            var messages = await messagesQuery
                .Where(m => m.To == user.Login)
                .ToListAsync();

            var viewModel = new UserMessagesViewModel
            {
                User = user,
                Messages = messages,
                Sender = sender,
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                SortOrder = sortOrder
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageText(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            message.Status = "read";
            await _context.SaveChangesAsync();

            return Json(new { text = message.Text, id = message.Id });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int userId, string recipient, string subject, string text)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var recipientUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == recipient);
            if (recipientUser == null)
            {
                ModelState.AddModelError("", "Получатель не найден");
                var messages = await _context.Messages.Where(m => m.To == user.Login).ToListAsync();
                return View("Messages", new UserMessagesViewModel { User = user, Messages = messages });
            }

            var message = new Message
            {
                From = user.Login,
                To = recipient,
                Subject = subject,
                Text = text,
                SentDate = DateTime.Now,
                Status = "new"
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Messages", new { userId = userId });
        }
    }
}