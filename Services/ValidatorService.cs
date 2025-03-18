namespace Bloc4_GUI.Services;

using System.Text.RegularExpressions;

public static class ValidatorService {


    public static bool isValidLandline(string Landline) {
        //Numéro de téléphone fixe, 01 à 05... 
        string pattern = @"^0[1-5]\d{8}$";

        return Regex.IsMatch(Landline.Replace(" ", ""), pattern);
    }

    public static bool isValidMobilePhoneNumber(string PhoneNumber) {
         
        //Numéro de téléphone portable au format national et international (0X ou +33)
        string pattern = @"^(\+33\s?|0)[67]\d{8}$";

        return Regex.IsMatch(PhoneNumber.Replace(" ", ""), pattern);
    }


    public static bool isValidEmail(string Email) {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        return Regex.IsMatch(Email.Replace(" ", ""), pattern);
    }


    public static bool isValidName(string Name) {
        // Entre 2 et 50 caractères, avec apostrophe, tiret..
        string pattern = @"^[A-Za-zÀ-ÿà-ÿ-' ]{2,50}$";

        return Regex.IsMatch(Name.Replace(" ", ""), pattern);
    }




}