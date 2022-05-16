
import {  Container } from 'semantic-ui-react';
import NavBar from './NavBar';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import { observer } from 'mobx-react-lite';
import { Route, Switch, useLocation } from 'react-router-dom';
import HomePage from '../../features/home/HomePage';
import ActivityForm from '../../features/activities/form/ActivityForm';
import { Fragment } from 'react';
import ActivityDetails from '../../features/activities/details/ActivityDetails';
import TestErrors from '../../features/errors/TestError';
import {ToastContainer} from 'react-toastify';
import NotFound from '../../features/errors/NotFounc';
import ServerError from '../../features/errors/ServerError';
import LoginForm from '../../features/users/LoginForm';
import { useStore } from '../stores/store';
import { useEffect } from 'react';
import LoadingComponent from './LoadingComponent';
import ModalContainer from '../models/modalContainer';
import ProfilePage from '../../features/profiles/ProfilePage';

function App() {
  const location = useLocation();
  const {commonStore, userStore} = useStore();
  useEffect(()=> {
    if(commonStore.token){
      userStore.getUser().finally(()=> commonStore.setAppLoaded());
    }
    else{
      commonStore.setAppLoaded();
    }
  },[commonStore,userStore]);
  if(!commonStore.appLoaded) return <LoadingComponent content='Loading app...' />
  return (
    <Fragment> 
      <ToastContainer position ="bottom-right" hideProgressBar />
      <ModalContainer />
      <Route exact path='/' component={HomePage} />
      <Route 
      path={'/(.+)'}
      render ={()=>(
        <>
        <NavBar />
        <Container style={{marginTop: '7em'}}>
          <Switch>
                    <Route path='/' exact component={HomePage}></Route>
        <Route exact path='/activities' component={ActivityDashboard}></Route>
        <Route path='/activities/:id' component={ActivityDetails}></Route>
        <Route key={location.key} path={['/createActivity','/manage/:id']} component={ActivityForm}></Route>
        <Route path='/profiles/:username' component={ProfilePage}/>
        <Route path='/errors' component={TestErrors}/>
        <Route path='/server-error' component={ServerError}/>
        <Route path='/login' component={LoginForm}/>
        <Route component={NotFound} />
        </Switch>

        </Container>
        </>

      )}
      />
      </Fragment>
  );
  //location.key is use to clear al the data from edit form when we navigate fron edit to create.
}

export default observer (App); 
 