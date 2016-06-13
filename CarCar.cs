using System;

public class CarCar : Car
{
    public void Decorate(bool isFront)
    {
        backAxis = transform.Find ("backAxis");
        if (isFront)
            frontAxis = transform.Find ("frontAxis");
    
    }


}
