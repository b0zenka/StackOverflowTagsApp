import { Routes } from '@angular/router';
import { TagListPageComponent } from './features/tags/pages/tag-list-page/tag-list-page.component';

export const routes: Routes = [
  { path: '', component: TagListPageComponent },
  { path: '**', redirectTo: '' },
];
