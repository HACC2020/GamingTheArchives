import User from '../models/user';
import { UserType } from '../models/user-type';

export const USERS: User[] = [
    {   EmailAddress: 'zzy@gmail.com',
        DisplayName: 'madMelvin',
        Type: UserType.Rookie,
        LastLogin: new Date(),
        SignUpDate: new Date(),
        Id: 11
    },

    {   EmailAddress: 'appy@gmail.com',
        DisplayName: 'Mary Poppings',
        Type: UserType.Rookie,
        LastLogin: new Date(),
        SignUpDate: new Date(),
        Id: 12
    },

    {   EmailAddress: 'unto@gmail.com',
        DisplayName: 'Furry Rabbit',
        Type: UserType.Rookie,
        LastLogin: new Date(),
        SignUpDate: new Date(),
        Id: 13
    },

    {   EmailAddress: 'pizzapie@gmail.com',
        DisplayName: 'ChiliMac',
        Type: UserType.Rookie,
        LastLogin: new Date(),
        SignUpDate: new Date(),
        Id: 14
    }


];
