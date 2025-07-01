using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static retail.Pages.PageLike;

namespace retail.AppData
{
    public static class CurrentUser
    {
        public static users Users { get; set; }
        public static List<favourites> Favorites { get; } = new List<favourites>();
        //public static BindingList<CartItem> Cart { get; } = new BindingList<CartItem>();
    }
}
