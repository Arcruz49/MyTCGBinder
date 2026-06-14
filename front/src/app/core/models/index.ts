export type CardVariant =
  | 'Normal'
  | 'Holo'
  | 'ReverseHolo'
  | 'Promo'
  | 'FullArt'
  | 'IllustrationRare'
  | 'SpecialIllustrationRare'
  | 'HyperRare'
  | 'SecretRare';

export interface CardResponse {
  id: string;
  tcgCardId: string;
  name: string;
  number: string;
  setId: string;
  setName: string;
  rarity: string;
  imageSmall: string;
  imageLarge: string;
  variant: CardVariant;
  quantity: number;
}

export interface TcgCardResponse {
  id: string;
  name: string;
  number: string;
  setId: string;
  setName: string;
  rarity: string;
  imageSmall: string;
  imageLarge: string;
}

export interface TcgSetResponse {
  id: string;
  name: string;
  series: string;
  logo: string;
}

export interface PagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

export interface UserDto {
  name: string;
  email: string;
  token: string;
}

export interface AuthUser {
  id: string;
  name: string;
}
