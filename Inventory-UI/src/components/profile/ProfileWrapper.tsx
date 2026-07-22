import React from 'react';
import { useParams } from 'react-router-dom';
import Profile from './Profile';

interface ProfileWrapperProps {
    children: (params: { [key: string]: string | undefined }) => React.ReactNode;
}

const ProfileWrapper: React.FC = () => {
    const params = useParams<{userid:string}>();
    console.log("ProfileWrapper params:", params);
    return <Profile/>;
};

export default ProfileWrapper;