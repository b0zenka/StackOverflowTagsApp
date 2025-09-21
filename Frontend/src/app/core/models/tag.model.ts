export interface Tag {
  name: string;
  count: number;
  share: number;
}

export interface TagResponse {
  data: Tag[];
  page: number;
  size: number;
  total: number;
}
