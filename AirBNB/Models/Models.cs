namespace Models
{
    using System;
    using System.Collections.Generic;

    public class Types
    {
        public int type_id { get; set; }
        public string name_type { get; set; }
    }

    public class Camping_spots
    {
        public int spot_id { get; set; }
        public string spot_name { get; set; }
        public int price_night { get; set; }
        public string type_name { get; set; }
        public int place_id { get; set; }

        
    }

    public class availability
    {
        public int avb_id { get; set; }
        public int spot_id { get; set; }
        public DateOnly date_start { get; set; }
        public DateOnly date_end { get; set; }
        public int cur_ind {  get; set; }
    }

    public class Users
    {
        public int user_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public byte[] pic_data { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string password { get; set; }
        public bool is_owner { get; set; }
        public string content_type {  get; set; }
    }
    public class UsersPicture
    {
        public int user_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string password { get; set; }
        public bool is_owner { get; set; }
    }


    public class Reservations
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int spot_id { get; set; }
        public int price_total { get; set; }
        public int place_id { get; set; }
        public DateOnly date_start { get; set; }
        public DateOnly date_end { get; set; }

    }

    public class Location
    {
        public int location_id { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string zipcode { get; set; }
        public string full_address { get; set; }
    }

    public class Campingplace
    {
        public int place_id { get; set; }
        public int owner_id { get; set; }
        public int location_id { get; set; }
        public string place_description { get; set; }
        public string name { get; set; }
        public TimeSpan check_in { get; set; }
        public TimeSpan check_out { get; set; }
    }

    public class Picture
    {
        public int pic_id { get; set; }
        public string file_name { get; set; }
        public byte[] data { get; set; }
        public int place_id { get; set; }
        public string content_type { get; set; }
       
    }
    public class Reviews
    {
        public int review_id { get; set; }
        public int user_id { get; set; }
        public int place_id { get; set; }
        public string description { get; set; }
        public DateOnly date_posted { get; set; }
        public int stars { get; set; }
    }

    public class Amenities_only
    {
        public int am_id { get; set; }
        public string amenity { get; set; }
    }

    public class Amenities_camp
    {
        public int am_camp_id { get; set; }
        public int am_id { get; set; }
        public int place_id { get; set; }
    }
    
    public class UpdatedUser
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public bool is_owner { get; set; }
    }


    public class FilterMian
    {
        public decimal MaxPrice { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}

