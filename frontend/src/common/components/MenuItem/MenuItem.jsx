import { Link, useLocation } from "react-router-dom";
import { useEffect, useState } from "react";
import clsx from 'clsx';
import styles from './MenuItem.module.scss';

const MenuItem = ({ to, children }) => {
  const location = useLocation();
  const [isActive, setIsActive] = useState(false);
  
  useEffect(() => {
    const active = location.pathname === to || 
                  (to !== '/' && location.pathname.startsWith(to));
    setIsActive(active);
  }, [location, to]);
  
  return (
    <Link
      to={to}
      className={clsx(styles.link, isActive ? styles.link_active : styles.link_inactive)}
    >
      {children}
    </Link>
  );
};

export default MenuItem;