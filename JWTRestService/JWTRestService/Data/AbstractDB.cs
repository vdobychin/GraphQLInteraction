using JWTRestService.Models;
using System;
using System.Collections.Generic;

namespace JWTRestService.Data
{
    //Типа БД
    static class AbstractDB
    {
        //Типа БД
        public static List<Person> people = new List<Person>
        {
            new Person { Id = Guid.NewGuid(), Login="admin@gmail.com", Password="12345", Role = "admin" },
            new Person { Id = Guid.NewGuid(), Login="qwerty@gmail.com", Password="55555", Role = "user" }
        };
    }
}
