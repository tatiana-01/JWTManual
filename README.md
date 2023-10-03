
# JWTManual

Este proyecto implementa de una manera manual el uso de tokens para validacion de login, implemanetando encriptacion AES sin hacer uso de la libreria de json web token en .Net.

El funcionamiento de este método de creación y uso de tokens implica el siguiente proceso: cuando un usuario ingresa sus credenciales para iniciar sesión en la aplicación, se genera un token de corta duración, válido por 30 segundos. Este token se utiliza para autenticar la solicitud inicial de inicio de sesión. Posteriormente, cuando el usuario es autenticado correctamente, se genera un token de autenticación de larga duración, válido por 18 horas.

Este token de autenticación se utiliza para todas las consultas y operaciones dentro de la aplicación. La fecha de creación y fecha de vencimiento del token, se almacena en la base de datos. Si el usuario cierra sesión, la fecha de vencimiento de este token se actualiza para que coincida con el momento del cierre de sesión, invalidando así el token y requiriendo una nueva autenticación para futuras operaciones.



## Roadmap

Para iniciar este proyetco se debe tener como base un proyecto 4 capas en .Net con una base de datos que contenga la tabla usuarios con atributos: Id, Fecha alta, Email, Password y Fecha baja.


- Creación metodos de encriptacion y servicios de usuario

- Creacion controlador con edpoints

- Prueba de uso del token


## Creación metodos de encriptacion y servicios de usuario

Para iniciar creamos en api una carpeta userService donde vamos a guardar todos los métodos para realizar el token:

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/creacionUserService.png)


Los métodos que vamos a utilizar las declaramos en la interfaz de la siguiente manera:

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/interfaceUserService.png)


En la clase userService implementamos los métodos, esta interfaz y clase deben añadir en el scope del program actualizando la clase extensions de la siguiente manera:

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/extensionsUserService.png)


Para iniciar con la clase userService debemos declarar dos llaves que vamos a usar para encriptar la información y poder obtener el token, estas claves deben ser seguras se puede usar un generador automático de claves como el siguiente: https://www.roboform.com/es/password-generator , como vamos a usar AES-256 para la encriptación estas llaves deben ser mínimo de 32 bytes 

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService1.png)


El primer método que vamos a crear es encriptar de tipo sha256 un texto

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService2.png)


Este método convierte la cadena de texto en un arreglo de bytes utilizando ASCIIEncoding, y luego se calcula el hash SHA-256 de esos bytes utilizando el método ComputeHash de SHA256, para después convertir a decimal con strongbuilder y el ciclo for, por último se convierte a string y se retorna.

Después creamos los métodos para encriptar y desencriptar con las llaves que declaramos anteriormente


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService3.png)


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService4.png)


En estos métodos vamos a utilizar el algoritmo de cifrado AES-256, para encriptar vamos hacer los siguiente:
-	Se crea un vector de inicialización (IV) de 16 bytes. (El IV es un valor aleatorio que se utiliza junto con la clave para aumentar la seguridad del cifrado.)
-	La clave de encriptación se convierte de una cadena de texto a un arreglo de bytes utilizando UTF-8 encoding y se asigna a la propiedad Key del objeto Aes.
-	Se crea un objeto ICryptoTransform utilizando la clave y el IV. Este objeto se utiliza para realizar el cifrado.
-	Hacemos uso de memoryStream para crear un flujo de memoria para almacenar los datos cifrados temporalmente
-	Después de que todo el texto se ha cifrado y almacenado en el flujo de memoria, se convierte el flujo de memoria en un arreglo de bytes
-	El arreglo de bytes cifrados se convierte en una cadena de texto en formato Base64 y se devuelve como resultado del método.

Para el método de desencripta se realiza de la siguiente manera:
-	Convertir la cadena de texto cifrada, que está en formato Base64 a un arreglo de bytes.
-	La clave de desencriptación se convierte de una cadena de texto a un arreglo de bytes utilizando UTF-8 encoding y se asigna a la propiedad Key del objeto Aes.
-	Se crea un flujo de cifrado que utilizando el flujo de memoria, el transformador de desencriptación y el modo de lectura. Todo lo que se lee de este flujo de cifrado se desencripta.
-	Se crea un lector de texto que utiliza el flujo de cifrado. Esto permite leer el texto desencriptado del flujo de cifrado.
-	El texto desencriptado se lee del flujo de cifrado y se devuelve como resultado del método.

Opcionalmente podemos crear el método:

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService5.png)

Este método reemplaza todas las apariciones de "%2F" con "/". En las URL, "%2F" es una codificación para el carácter de barra inclinada ("/"). Entonces, esta función se utiliza para corregir la codificación de un token en una URL, convirtiendo "%2F" de nuevo en la barra inclinada ("/").

El siguiente método registra los usuarios en la base de datos:

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService6.png)

Este método recibe un objeto de tipo usuario, de donde obtenemos los datos de registro, encriptamos la contraseña con el método de getSHA256, revisamos si ese usuario ya ha sido registrado, si no se encuentra registrado se añade a la base de datos y se guardan los cambios y se envía un mensaje dependiendo del resultado.


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService7.png)

En el método de get token login se obtiene la fecha actual como string en el formato que especificamos, para encriptar la información del usuario utilizando el método anterior, donde pasamos la key para login y un string que es la unión de la fecha, el email y la contraseña todo unido con #.


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService8.png)

En el metodo login by token recibe el token generado anteriormente, los desencripta para obtener los datos de fecha, email y contraseña, verifica que no haya pasado mas de 30 segundos desde que se creo ese token restando la fecha actual con la fecha desencriptada, y retorna un -1 si se excede este tiempo.
Si no lo excede continua con el metodo donde se revisa que el email y la contraseña sean correcta, si estan bien se crea el token  con la user key y la informacion del email y fecha de creacion, ademas la fechas de creacion y vencimiento se almacenan en la base de datos y se retorna el token, si el email y la contraseña no coincide se retorma un -2, y si ocurre algun error se envia como respuesta un -3.


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService9.png)

Este metodo obtiene el email del token de usuario que se genera despues del login, se desencripta y se separa string por el #   ya que este fue el que se utilizo al momento de realizar la encriptacion y se toma la posicion 0.


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService10.png)

En este metodo se verifica si el token no se ha expirado, se corrige el token , se desencripta y se obtiene el email y la fecha, se hace una busqueda a la base datos para traer la informacion del usuario, se hace una conversion de fecha desencriptada para poder hacer una correcta comparacion, se compara la fecha actual con la fecha desencriptada y la fecha de vencimiento al macenada en la base de datos, si la fecha actual es mayor a alguna de las fecha anteriores retorna falso lo cual indica que no es valido el token, de lo contrario retorna true.


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService11.png)

Este metodo permite cambiar la contraseña, para esto debe recibir un token junto con la contraseña antigua y la nueva contraseña, se encriptan estos datos para poder comparar la contraseña almacenada en la base datos con la contraseña que se ingreso, si coinciden se asigna la nueva contraseña encriptada, y se retorna true  avidsando un correcto cambio de contraseña de lo contrario retorna false.


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/repoUserService12.png)

Este método invalida el token cuando se cierra sesión, para esto debe recibir el token en cual verifica si está activo, si lo esta se desencripta este token y con email se busca la información del usuario de la base de datos para actualizar la fecha de vencimiento la cual pasa a ser la de momento de cierre de sesión y retorna true asegurando el cambio de contraseña.

## Creacion controlador con edpoints
Teniendo los métodos anteriores se pueden realizar los controladores de la siguiente manera:

Creamos un base api controller donde estará a ruta genérica

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/baseApiController.png)


Después creamos el usuario controller con los endpoints que deseamos utilizar 

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/usuarioController.png)

Al inicio del controlador hacemos referencia a las clases que vamos a utilizar como userService, UnitOfWork y mapper, además se crea el constructor asignando las referencias anteriores


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/usuarioController1.png)


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/usuarioController2.png)


En el endpoint de registro utilice un dto para que el usuario envie solo la informacion necesaria, este dto se realiza en la carpeta dtos y se hace uso de automapper, tambien se debe crear una clase mapping profiles para su correcto funcionamiento.

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/dto.png)


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/profiles.png)

## Prueba de uso del token
El funcionamiento de estos endpoints es el siguiente:
- Se registra un usuario:

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/postAgregar.png)


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/agregarRta.png)


- Se ingresa datos de login y se obtiene el token de login

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/postGetTokenLogin.png)


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/GetTokenLoginRta.png)


- Se ingresa el token de login en el endpoint loginByToken, donde se obtiene el token de usuario

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/postLoginByToken.png)


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/loginByTokenRta.png)


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/bdInicioSesion.png)

En la base de datos podemos observar que asigna la fecha de creación y de vencimiento

- Con el token anterior se puede usar para cambiar contraseña y/o cerrar sesión
Para el cambio de contraseña solo se envía los datos que pide: la contraseña antigua, la nueva y el token

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/postSetPassword.png)


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/setPasswordRta.png)


Y para cerrar sesión solo se debe ingresar el token

![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/postLogout.png)


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/logoutRta.png) 


![photo](https://github.com/tatiana-01/JWTManual/raw/main/images/bdLogout.png)

Aquí ya podemos ver como la fecha de vencimiento cambia y este token se invalida.
## Acknowledgements
Este proyecto se encuentra basado en el trabajo: 
[tosblama/mtdev_LoginTokenPropio_ASPNetWebAPI.git](https://github.com/tosblama/mtdev_LoginTokenPropio_ASPNetWebAPI.git)

Temas de consulta para el proyecto: 
[AES](https://www.pandasecurity.com/es/mediacenter/tecnologia/cifrado-aes-guia/ )


