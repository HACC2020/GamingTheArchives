import { $enum } from 'ts-enum-util';

export enum UserType {
  Rookie,
  Indexer,
  Proofer,
  Archivist,
  Administrator
}

export function toUserType(value: any): UserType {
  if (typeof value === 'string') {
    return UserType[$enum(UserType).asKeyOrDefault(value)];
  } else if (typeof value === 'number') {
    return value as UserType;
  }
}
