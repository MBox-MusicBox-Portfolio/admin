// Документация : https://joi.dev/api/?v=17.9.1#introduction
import Joi from 'joi';
import crypto from 'bcrypt';

const response={success:false, value:{}}

/**
 * Shema object for registration 
 * Схема валидации для регистрации 
 */
export const registration = Joi.object({
    Name: Joi.string().alphanum().empty().required(),
    Email: Joi.string().email({minDomainSegments: 2, tlds: {allow:['com','net']}}).required(),
    Birthday: Joi.number().integer().min(1900).max(new Date().getFullYear()),
    Password: Joi.string().min(4).max(30).empty().required(),
});

/**
 * Shema object for authorization
 * Схема валидации для авторизации 
 */
export const authorization= Joi.object({
    Email: Joi.string().email().required(),
    Password: Joi.string().empty().required()
});

/**
 * Shema object for forgotten pass form validation 
 * Схема валидации для формы забытых паролей пользователей
 */
export const forgotten = Joi.object({
    Password: Joi.string().empty().min(4).max(8).required(),
    RePassword: Joi.string().empty().required() 
});

export async function getValidationAuthorization(formobject)
{
    if(formobject)
    {
         const auth = authorization.validate(formobject,{abortEarly:false});
        return auth;
    }else{
        console.error("Validator error: Empty object");
        throw new error;  
    }
}

export async function getValidationRegistration(formobject)
{
    if(formobject)
    {
         const shemaRegistryValidation = registration.validate(formobject,{abortEarly:false});
        return shemaRegistryValidation;
    }else{
        console.error("Validator error: Empty object");
    }
    return null; 
}

export async function getValidationForgottenPassword(formobject)
{
    if(object){
        const forgottenPass = forgotten.validate(object,{abortEarly:false}); 
        return forgottenPass;
    }else{
        console.error("Validator error: Null object!");
    }
}
/**
 * Splits a single string containing name, email, and password into separate strings.
 * Object with separate name, email and password properties.
 * Разбивает цельную строку ошибок name,email и password на отдельные строки
 * Возвращает объект со значением полей для имени, электронной почты и пароля.
 * @param {*} errorString 
 * @returns 
 */
export async function parserErrorString(errorString)
{
   if(errorString)
   { 
    let objectString = {}
    for (const error of errorString.error.message.trim().split('.')) 
    {
      if (error.includes("Name")) {
           objectString.Name  = error.replace(/"/g, "").trim();
      } else if (error.includes("Email")) {
           objectString.Email = error.replace(/"/g, "").trim();
      } else if (error.includes("Password")) {
           objectString.Password = error.replace(/"/g, "").trim();
      } else if(error.includes("RePassword")){
           objectString.RePassword = error.replace(/"/g,"").trim();
      }
    }
            response.value = await hideEmptyObjectProperty(objectString);
          objectString = {}; 
      return response;
   }
 }

/**
* Searches for empty values in the properties of an object and hides them.
* Ищет пустые значения в свойствах объекта и скрывает их в респонсе.
*/
export async function hideEmptyObjectProperty(objectString)
{
  let result={} 
  if(objectString){
    Object.entries(objectString).forEach(([key, value]) =>{
        if (key.includes("Name") && value !== ''){
            result.Name = value;
        }else if(key.includes("Email") && value !== ''){
            result.Email = value;
        }else if(key.includes("Password") && value !== '')
        {
            result.Password = value;
        }
     });
     return result;
    }
} 

/**
 * Валидатор базы данных
 * Database validation
 * @param {*} object   Тело запроса . Request body
 * @param {*} user     Сузщность БД . Sequelize database object
 * @param {*} context  Контекст сервера . Server context 
 * @returns 
 */
export async function databaseValidator(object , user,context)
{
    if (!user) {
        context.status = 404;
            response.value = {email: "User doesn't exist"};
        return response;
    }else{
        if(user.Password !== crypto.hashSync(object.Password, process.env.PASS_SALT))
        {
            context.status = 401;
               response.value = {password: "Invalid password"};
            return response;
        }else if(!user.IsEmailVerify)
        {
            context.status = 202;
                response.value = {
                  email: "To complete the registration process. Your email must be confirmed!"
         };
            return response;
        }else if(user.IsBlocked){
              context.status = 403;
            response.value={
                 email:"Your account has been suspended due to a violation of our rules and policies. For further information, please contact us."
            }
            return response;
        }else{
              context.status = 200;
            return true; 
        }
    }
}