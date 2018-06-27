using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using BrightIdeas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace BrightIdeas.Controllers
{
    public class HomeController : Controller
    {
        //Context setup
        private Context _context;

        public HomeController(Context context)
        {
            _context = context;
        }
        
        //Login and Register page
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        //Register process
        [HttpPost("Register")]
        public IActionResult Register(Users users, string CPassword)
        {
            if (ModelState.IsValid)
            {
                if(users.password == CPassword)
                {
                    //Create new user
                    System.Console.WriteLine("Passwords match!---------------------");
                    PasswordHasher<Users> Hasher = new PasswordHasher<Users>();
                    users.password = Hasher.HashPassword(users, users.password);
                    _context.Add(users);
                    _context.SaveChanges();

                    //Save session Use
                    int? signIn = HttpContext.Session.GetInt32("LoggedInUser");
                    HttpContext.Session.SetInt32("LoggedInUser", (int)users.id);
                    return RedirectToAction("Success");
                }
                else
                {

                    return View("Index");
                }
            }
            else
            {
                System.Console.WriteLine("Error---------------------");
                return View("Index");
            }
            
        }

        //Login success or fail
        [HttpPost("Login")]
        public IActionResult Login(string Email, string PasswordToCheck)
        {
            // Attempt to retrieve a user from your database based on the Email submitted
            Users users = _context.users.SingleOrDefault(login=>login.email == Email);
            System.Console.WriteLine(users);
            if (users != null && PasswordToCheck != null)
            {
                var Hasher = new PasswordHasher<Users>();
                if (0 != Hasher.VerifyHashedPassword(users, users.password, PasswordToCheck))
                {
                    //Save session Use
                    int? signIn = HttpContext.Session.GetInt32("LoggedInUser");
                    HttpContext.Session.SetInt32("LoggedInUser", (int)users.id);

                    System.Console.WriteLine("Password Match");
                    return RedirectToAction("Success");
                }
                else
                {
                    return View("Index");
                }
                // Pass the user object, the hashed password, and the PasswordToCheck
            }
            //Handle failure
            else
            {
                return View("Index");
            }
            
        }
        

        //Success page
        [HttpGet("bright_ideas")]
        public IActionResult Success()
        {

            //Session User ID
            int? signIn = HttpContext.Session.GetInt32("LoggedInUser");
            ViewBag.signedIn = signIn;
            if (signIn == null)
            {
                System.Console.WriteLine("signIn is Null");
                return View("Index");
            }

            //Session User
            Users findByID = _context.users.SingleOrDefault(user => user.id == signIn);
            ViewBag.logUser = findByID;
            int sessionID = ViewBag.logUser.id;

            //All Posts
            List<Posts> allPosts = _context.posts.ToList();
            List<Users> allUsers = _context.users.ToList();
            List<Likes> allLikes = _context.likes.ToList();


            IEnumerable<Posts> sortedPosts =
                        from post in allPosts
                        orderby post.likes descending
                        select post;
            //Viewbags
            ViewBag.Posts = sortedPosts;
            ViewBag.Users = allUsers;
            ViewBag.Likes = allLikes;


            foreach(var post in allPosts)
            {
                //Count Established
                int count = 0;
                //Sorts through guests
                foreach(var likes in allLikes)
                {
                    if(post.id == likes.posts_id)
                    {
                        count+=1;
                    }
                }
                Posts aPost = _context.posts.SingleOrDefault(getPost => getPost.id == post.id);
                aPost.likes = count;
                _context.SaveChanges();
            }
            Users sessionUser = _context.users.SingleOrDefault(getUser => getUser.id == sessionID);
            ViewBag.sessionUserBag = sessionUser;
            System.Console.WriteLine(sessionUser);
            System.Console.WriteLine(sessionID);
            return View("Success");
        }


        [HttpGet("AddPost")]
        public IActionResult AddPostReRoute()
        {
            return RedirectToAction("Index");
        }
        [HttpPost("AddPost")]
        public IActionResult AddPost(Posts newPost)
        {
            //Session User ID
            int? signIn = HttpContext.Session.GetInt32("LoggedInUser");
            ViewBag.signedIn = signIn;
            if (signIn == null)
            {
                return View("Index");
            }
            //Session User
            Users findByID = _context.users.SingleOrDefault(user => user.id == signIn);
            ViewBag.logUser = findByID;
            int sessionID = ViewBag.logUser.id;

            //Add New post
            newPost.usersID = findByID.id;
            _context.Add(newPost);
            _context.SaveChanges();
            return RedirectToAction("Success");

        }

        [HttpGet("AddLike")]
        public IActionResult AddLikeReRoute()
        {
            return RedirectToAction("Index");
        }
        [HttpPost("AddLike")]
        public IActionResult AddLike(Likes newLike, int addLikePostID, int likeUserID)
        {
            List<Likes> allLikes = _context.likes.Where(like => like.posts_id == addLikePostID).ToList();
            foreach(var like in allLikes)
            {
                if(like.users_id == likeUserID)
                {
                    return RedirectToAction("Success");
                }
            }

            //Session User ID
            int? signIn = HttpContext.Session.GetInt32("LoggedInUser");
            ViewBag.signedIn = signIn;
            if (signIn == null)
            {
                return View("Index");
            }


            newLike.users_id = likeUserID;
            newLike.posts_id = addLikePostID;
            _context.Add(newLike);
            _context.SaveChanges();
            return RedirectToAction("Success");
        }
        [HttpGet("Likes")]
        public IActionResult LikeReRoute()
        {
            return RedirectToAction("Index");
        }

        [HttpPost("Likes")]
        public IActionResult Likes(int viewLikePostID)
        {
            //Session User ID
            int? signIn = HttpContext.Session.GetInt32("LoggedInUser");
            ViewBag.signedIn = signIn;
            if (signIn == null)
            {
                return View("Index");
            }

            Posts aPost = _context.posts.SingleOrDefault(getPost => getPost.id == viewLikePostID);
            Users aUser = _context.users.SingleOrDefault(getUser => getUser.id == aPost.usersID);
            List<Likes> theLikes = _context.likes.Where(getLike => getLike.posts_id == aPost.id).ToList();
            List<Users> allUsers = _context.users.ToList();


            //ViewBags
            ViewBag.theLikes = theLikes;
            ViewBag.allUsers = allUsers;
            ViewBag.aPost = aPost;
            ViewBag.aUser = aUser;
            return View("Likes");
        }

        [HttpGet("UserProfile")]
        public IActionResult ProfileReRoute()
        {
            return RedirectToAction("Index");
        }

        [HttpPost("UserProfile")]
        public IActionResult UserProfile(int userID)
        {
            List<Posts> thePosts = _context.posts.ToList();
            Users aUser = _context.users.SingleOrDefault(getUser => getUser.id == userID);
            List<Likes> theLikes = _context.likes.Where(getLike => getLike.users_id == userID).ToList();

            //Session User ID
            int? signIn = HttpContext.Session.GetInt32("LoggedInUser");
            ViewBag.signedIn = signIn;
            if (signIn == null)
            {
                return View("Index");
            }
            

            if(thePosts != null)
            {
                int count = 0;
                foreach(var post in thePosts)
                {
                    if(post.usersID == aUser.id)
                    {
                        count+=1;
                    }
                }
                ViewBag.postCount = count;
            }

            if(theLikes!= null)
            {
                int count = 0;
                foreach(var like in theLikes)
                {
                    count+=1;
                }
                ViewBag.likeCount = count;
            }
            //ViewBags
            ViewBag.aUser = aUser;
            return View("UserProfile");
        }


        [HttpGet("Delete")]
        public IActionResult DeleteReRoute()
        {
            return RedirectToAction("Index");
        }
        [HttpPost("Delete")]
        public IActionResult Delete(int deletePostID)
        {
            //Delete
            Posts deletePost = new Posts { id = deletePostID };
            _context.Remove(deletePost);
            _context.SaveChanges();
            return RedirectToAction("Success");
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
