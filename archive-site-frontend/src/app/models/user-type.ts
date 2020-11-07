import { $enum } from 'ts-enum-util';

export enum UserType {
  Rookie = 'Rookie',
  Indexer = 'Indexer',
  Proofer = 'Proofer',
  Archivist = 'Archivist',
  Administrator = 'Administrator'
}

export function toUserType(value: string): UserType {
  return UserType[$enum(UserType).asKeyOrDefault(value)];
}
