import { MessagesResolver } from './_resolvers/messages.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { MemberDetailResolver } from './_resolvers/member-detail-resolver';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { HomeComponent } from './home/home.component';
import {Routes} from '@angular/router';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberEditComponent } from './members/member-edit/member-edit.component';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            {path: 'messages', component: MessagesComponent ,
            resolve: {messages: MessagesResolver}},
            {path: 'members', component: MemberListComponent},
            {path: 'members/:id', component: MemberDetailComponent,
            resolve: {user: MemberDetailResolver}},
            {path: 'member/edit', component: MemberEditComponent,
            resolve: {user: MemberEditResolver}, canDeactivate: [PreventUnsavedChanges]},
            {path: 'lists', component: ListsComponent},
        ]
    },
    {path: '**', redirectTo: '', pathMatch: 'full'},

];

